using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(Button))]
//estos se deben auto actualizar con las unidades que tienen vinculadas
public class ReinforcementButton : MonoBehaviour
{
    private Spawner currentSpawner;
    private AIUnit linkedUnit;

    private Image imgComp;
    private Button buttonComp;

    private void Awake()
    {
        imgComp = GetComponent<Image>();
        buttonComp = GetComponent<Button>();

        buttonComp.onClick.AddListener(OnClickCallback);
    }

    public void UpdateButton(AIUnit unit, Spawner spawner, Sprite sprite)
    {
        linkedUnit = unit;
        currentSpawner = spawner;
        imgComp.sprite = sprite;
    }

    public void OnClickCallback()
    {
        if (TryReinforceUnit())
        {
            //no hacer nada.
        }
        else
        {
            Debug.Log($"can't spawn more units inside: {linkedUnit}");
            //mandar su mensaje
        }
    }

    private bool TryReinforceUnit()
    {        
        return currentSpawner.TrySpawnAgent(linkedUnit, out AIAgent agent);        
    }

}
