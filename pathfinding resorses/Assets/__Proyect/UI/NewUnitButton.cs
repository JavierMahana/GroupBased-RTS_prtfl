using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(Button))]
public class NewUnitButton : MonoBehaviour
{    
    private Image imgComp;
    private TextMeshProUGUI text;

    private AIUnitData currData;
    private Spawner currentSpawner;    
    private Button buttonComp;

    private void Awake()
    {
        buttonComp = GetComponent<Button>();

        buttonComp.onClick.AddListener(OnClickCallback);
    }

    public void UpdateButton(AIUnitData data, Spawner spawner)
    {
        currData = data;
        currentSpawner = spawner;
    }

    public void OnClickCallback()
    {
        if (TryCreateUnit())
        {
            //no hacer nada.
        }
        else
        {
            Debug.Log("No se pudo spawnear una unidad");
            //mandar su mensaje
        }
    }

    private bool TryCreateUnit()
    {
        if (currentSpawner.TrySpawnUnit(currData, out AIUnit newUnit))
        {
            if (currentSpawner.TrySpawnAgent(newUnit, out AIAgent newAgent))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}
