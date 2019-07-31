using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Lean.Pool; 

//esto la idea es que sea triggereado por una estructura.
//es parte del UI
//el nombre hay que cambiarlo
public class SpawnerList : SerializedMonoBehaviour
{
    public RectTransform rectToPlaceOptionList;
    public Vector2Int anchoredPos1;
    public Vector2Int buttonProportions;
    public int separation; 

    public List<AIUnitData> spawnableUnits = new List<AIUnitData>();
    [AssetsOnly]
    public UnitDataToDisplayButton dataToDisplayButton;

    public bool selected;


    private List<UnitDisplayButton> activeButons = new List<UnitDisplayButton>();

    public void Update()
    {
        if (selected)
        {

        }
    }
    [Button]
    private void HideSpawnableUnitsButtons()
    {
        foreach (UnitDisplayButton button in activeButons)
        {
            LeanPool.Despawn(button);
        }
        activeButons.Clear();
    }
    [Button]
    private void ShowSpawnableUnitsButtons()
    {
        for (int i = 0; i < spawnableUnits.Count; i++)
        {
            AIUnitData unitData = spawnableUnits[i];

            UnitDisplayButton newButton = LeanPool.Spawn(dataToDisplayButton.dictionary[unitData]);
            newButton.transform.SetParent(rectToPlaceOptionList, false);

            //setting the rect transform params
            RectTransform rTransform = newButton.GetComponent<RectTransform>();
            //anchors to the down left corner
            rTransform.anchorMax = rTransform.anchorMin = new Vector2(0, 0);
            //the position is the firs pos moved to the right by the proportions and separation param
            rTransform.anchoredPosition = anchoredPos1 + Vector2Int.right * ((buttonProportions.x + separation) * i);
            //the delta is the proportions because tha anchors are all in the same place.
            rTransform.sizeDelta = buttonProportions;

            activeButons.Add(newButton);
        }


    }
}

