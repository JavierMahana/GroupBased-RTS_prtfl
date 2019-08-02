using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//
public class UISpawnerManager : MonoBehaviour
{


    private void OnEnable()
    {
        Spawner.OnSelect += ShowUnitDisplayButtons;
    }

    
    private void ShowUnitDisplayButtons(Spawner requester)
    {
        List<AIUnitData> requesterList = requester.unitsToSpawn;

        int numOfButtonsToDisplay = requesterList.Count;

        for (int i = 0; i < numOfButtonsToDisplay; i++)
        {
            AIUnitData currentData = requesterList[i];
        }
    }
}
