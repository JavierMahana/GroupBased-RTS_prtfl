using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDisplayButton : MonoBehaviour
{
    public AIUnitData unitData;
    public Team team;

    
    public void ShowCreationButtons()
    {
        List<AIUnit> unitsToShow = UnitManager.Instance.FindUnits(unitData, team);

        if (unitsToShow != null)
        {
            foreach (AIUnit unit in unitsToShow)
            {
            }
        }
    }



    public void HideCreationButtons()
    {

    }
}
