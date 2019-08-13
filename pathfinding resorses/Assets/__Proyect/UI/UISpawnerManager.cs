using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;
using Sirenix.OdinInspector;
using Lean.Pool;

//renombrar a UISpawnerHandler
public class UISpawnerManager : MonoBehaviour
{
    public UnitManager unitManager;

    [Required]
    public UnitDataToSprite portraitDictionary;
    public RectTransform[] buttonPlaceHolders;
    [AssetsOnly]
    public RectTransform reinforcementButtonPrefab;
    [AssetsOnly]
    public RectTransform creationButtonPrefab;
    public NewUnitButton newUnitButton;


    private CreationButton[] creationButtons;
    private ReinforcementButton[] reinforcementButtons;

    private void Start()
    {
        SetUpCreationAndReinforcementButtons();
    }
    private void SetUpCreationAndReinforcementButtons()
    {
        #region asserts
        Debug.Assert(buttonPlaceHolders != null && buttonPlaceHolders.Length == 9, "set up the buttonPlaceHolders variable!");
        foreach (var placeholder in buttonPlaceHolders) { Debug.Assert(placeholder != null, "All the place holder slots must have a values!"); }
        Debug.Assert(reinforcementButtonPrefab != null && creationButtonPrefab != null, "The prefabs must be set");
        Debug.Assert(reinforcementButtonPrefab.GetComponentInChildren<ReinforcementButton>() && creationButtonPrefab.GetComponentInChildren<CreationButton>() != null, "The érfabs must have the buttons components in it!");
        #endregion
        int lenght = buttonPlaceHolders.Length;

        creationButtons = new CreationButton[lenght];
        reinforcementButtons = new ReinforcementButton[lenght];

        for (int i = 0; i < lenght; i++)
        {
            RectTransform placeHolder = buttonPlaceHolders[i];

            int count = placeHolder.childCount;
            bool reinfRequired = true;
            bool creatRequired = true;

            ReinforcementButton currRienfButt = null;
            CreationButton currCreatButt = null;

            for (int f = 0; f < count; f++)
            {
                Transform child = placeHolder.GetChild(f);

                if (reinfRequired)
                {
                    ReinforcementButton tempReinfButt = child.GetComponentInChildren<ReinforcementButton>();
                    reinfRequired = tempReinfButt == null;
                    if (!reinfRequired) { currRienfButt = tempReinfButt; }
                }
                if (creatRequired)
                {
                    CreationButton tempCreatButt = child.GetComponentInChildren<CreationButton>();
                    creatRequired = tempCreatButt == null;
                    if (!creatRequired) { currCreatButt = tempCreatButt; }
                }
            }

            if (reinfRequired)
            {
                RectTransform prefabInst = LeanPool.Spawn(reinforcementButtonPrefab);
                ReinforcementButton rf = prefabInst.GetComponentInChildren<ReinforcementButton>();
                currRienfButt = rf;

                RectTransform transf = (RectTransform)rf.transform;
                InitButton(placeHolder, prefabInst);


            }
            if (creatRequired)
            {
                RectTransform prefabInst = LeanPool.Spawn(creationButtonPrefab);
                CreationButton cr = prefabInst.GetComponentInChildren<CreationButton>();
                currCreatButt = cr;

                RectTransform transf = (RectTransform)cr.transform;
                InitButton(placeHolder, prefabInst);
            }

            creationButtons[i] = currCreatButt;
            reinforcementButtons[i] = currRienfButt;
        }
    }


    private void InitButton(Transform parentPlaceHolder, RectTransform rectTransform)
    {
        rectTransform.SetParent(parentPlaceHolder, false);

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = new Vector2(1, 1);

        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }

    private void OnEnable()
    {
        Spawner.OnSelect += ShowCreationView;
        CreationButton.OnCreationButtonClick += ShowReinforcementView;
    }
    private void OnDisable()
    {
        Spawner.OnSelect -= ShowCreationView;
        CreationButton.OnCreationButtonClick -= ShowReinforcementView;
    }
    //luego estos dos metodos seran publicos y llamados por el UISelectionManager
    private void ShowReinforcementView(Spawner spawner, AIUnitData data)
    {
        List<AIUnit> compatibleUnits = unitManager.FindUnits(data, spawner.team);

        HideReinforcementButtons();
        UpdateReinforcementButtons(spawner, data, compatibleUnits);
        newUnitButton.UpdateButton(data, spawner);



        ShowReinfocementButtons(compatibleUnits.Count);
    }

    private void HideReinforcementButtons()
    {
        foreach (ReinforcementButton button in reinforcementButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
    private void ShowReinfocementButtons(int count)
    {
        for (int i = 0; i < count; i++)
        {
            reinforcementButtons[i].gameObject.SetActive(true);
        }        
    }
    private void UpdateReinforcementButtons(Spawner spawner, AIUnitData data, List<AIUnit> compatibleUnits)
    {
        Debug.Assert(portraitDictionary.dictionary.TryGetValue(data, out Sprite buttonSprite), "update the portrait dictionary!");
        if (compatibleUnits == null) return;

        for (int i = 0; i < compatibleUnits.Count; i++)
        {
            AIUnit currUnit = compatibleUnits[i];
            ReinforcementButton currButton = reinforcementButtons[i];
            Debug.Assert(currUnit && currButton != null);



            currButton.UpdateButton(currUnit, spawner, buttonSprite);
        }
    }



    public void ShowCreationView(Spawner spawner)
    {
        HideCreationButtons();
        UpdateCreationButtons(spawner);
        ShowCreationButtons(spawner.spawnUnits.Count);
    }
    private void UpdateCreationButtons(Spawner spawner)
    {
        List<AIUnitData> spawnableUnits = spawner.spawnUnits;        

        for (int i = 0; i < spawnableUnits.Count; i++)
        {
            AIUnitData currentData = spawnableUnits[i];

            Debug.Assert(portraitDictionary.dictionary.TryGetValue(currentData, out Sprite buttonSprite), "update the portrait dictionary!");

            CreationButton currCreationButton = creationButtons[i];
            currCreationButton.UpdateButton(spawner, currentData, buttonSprite);
        }
    }
    private void ShowCreationButtons(int count)
    {
        for (int i = 0; i < count; i++)
        {
            creationButtons[i].gameObject.SetActive(true);
        }
    }
    private void HideCreationButtons()
    {
        foreach (CreationButton button in creationButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
}
