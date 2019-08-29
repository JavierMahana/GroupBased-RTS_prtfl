using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;
using Sirenix.OdinInspector;
using Lean.Pool;


//modo creación
//en el panel estatico estarán las posibilidades de creación de unidades y en el flexible estará la info de la estructura.
//modo refuerzo
//en el panel estatico estarán un boton de atras y de crear nueva unidad y en el flexible estaran las unidades a las que se pueden mandar refuerzos.
[RequireComponent(typeof(UISelectionMenuManager))]
public class UISpawnerManager : MonoBehaviour
{
    public UnitManager unitManager;
    public string showCreationUIEvent;
    public string showReinforcementUIEvent;

    [AssetsOnly]
    public RectTransform reinforcementButtonPrefab;
    [AssetsOnly]
    public RectTransform creationButtonPrefab;
    public NewUnitButton newUnitButton;

    private UISelectionMenuManager selectionMenuManager;

    private CreationButton[] creationButtons;
    private ReinforcementButton[] reinforcementButtons;

    #region initialization
    private void Start()
    {
        selectionMenuManager = GetComponent<UISelectionMenuManager>();
        SetUpCreationAndReinforcementButtons();
    }
    private void SetUpCreationAndReinforcementButtons()
    {
        #region asserts        
        Debug.Assert(reinforcementButtonPrefab != null && creationButtonPrefab != null, "The prefabs must be set");
        Debug.Assert(reinforcementButtonPrefab.GetComponentInChildren<ReinforcementButton>() != null && creationButtonPrefab.GetComponentInChildren<CreationButton>() != null, "The prefabs must have the buttons components in it!");
        #endregion
        int lenght = selectionMenuManager.staticGridPanelPlaceHolders.Length;

        creationButtons = new CreationButton[lenght];
        reinforcementButtons = new ReinforcementButton[lenght];

        for (int i = 0; i < lenght; i++)
        {
            RectTransform placeHolder = selectionMenuManager.staticGridPanelPlaceHolders[i];

            int count = placeHolder.childCount;
            bool reinfRequired = true;
            bool creatRequired = true;

            ReinforcementButton currRienfButt = null;
            CreationButton currCreatButt = null;

            //for each placeholder instantiate a button if it isn't one yet.
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
                InitButtonPosition(placeHolder, prefabInst);


            }
            if (creatRequired)
            {
                RectTransform prefabInst = LeanPool.Spawn(creationButtonPrefab);
                CreationButton cr = prefabInst.GetComponentInChildren<CreationButton>();
                currCreatButt = cr;

                RectTransform transf = (RectTransform)cr.transform;
                InitButtonPosition(placeHolder, prefabInst);
            }

            creationButtons[i] = currCreatButt;
            reinforcementButtons[i] = currRienfButt;
        }
    }
    private void InitButtonPosition(Transform parentPlaceHolder, RectTransform rectTransform)
    {
        rectTransform.SetParent(parentPlaceHolder, false);

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = new Vector2(1, 1);

        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }
    #endregion

    public void ShowCreationView(Spawner spawner)
    {
        GameEventMessage.SendEvent(showCreationUIEvent);

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

            Debug.Assert(selectionMenuManager.portraitDictionary.dictionary.TryGetValue(currentData, out Sprite buttonSprite), "update the portrait dictionary!");

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

    public void ShowAndSetReinforcementView(Spawner spawner, AIUnitData data)
    {
        spawner.SetReinforcementMode(data, unitManager);
        ShowReinforcementView(spawner);
    }
    public void ShowReinforcementView(Spawner spawner)
    {
        GameEventMessage.SendEvent(showReinforcementUIEvent);

        HideReinforcementButtons();
        UpdateReinforcementButtons(spawner, spawner.currentUnitData);
        ShowReinfocementButtons(spawner.currentReinforcementCompatibleUnits.Count);
        newUnitButton.UpdateButton(spawner.currentUnitData, spawner);
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
    private void UpdateReinforcementButtons(Spawner spawner, AIUnitData data)
    {
        Debug.Assert(selectionMenuManager.portraitDictionary.dictionary.TryGetValue(data, out Sprite buttonSprite), "update the portrait dictionary!");
        if (spawner.currentReinforcementCompatibleUnits == null) return;

        for (int i = 0; i < spawner.currentReinforcementCompatibleUnits.Count; i++)
        {
            AIUnit currUnit = spawner.currentReinforcementCompatibleUnits[i];
            ReinforcementButton currButton = reinforcementButtons[i];
            Debug.Assert(currUnit && currButton != null);



            currButton.UpdateButton(currUnit, spawner, buttonSprite);
        }
    }



    private void OnEnable()
    {
        CreationButton.OnCreationButtonClick += ShowAndSetReinforcementView;
    }
    private void OnDisable()
    {
        CreationButton.OnCreationButtonClick -= ShowAndSetReinforcementView;
    }
}
