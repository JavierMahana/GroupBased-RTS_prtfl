using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image), typeof(Button))]
public class UIStaticGridButton : MonoBehaviour
{
    private Button button;
    private Image image;

    private Spawner spawner;
    private AIUnitData spawnerSpawnUnit;

    private AIUnit unit;
    private Builder builder;

    private UIMode currentButtonMode
    {
        get
        {
            if (unit != null) return UIMode.UNIT;
            else if (builder != null) return UIMode.BUILDER;
            else if (spawner != null) return UIMode.SPAWNER;
            else return UIMode.OTHER;
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button.onClick.GetPersistentEventCount() > 0) Debug.LogWarning($"this button: {this} has persistent events, this may not be wanted");
        button.onClick.AddListener(OnButtonPressed);

        image = GetComponent<Image>();
    }
    //el boton al ser apretado debe saber que hacer.
    //en el caso de ser apretado en por un spawner. Este debe modificar al spawner al cual esta vinculado.

    //por ahora los botones van ha hacer mucha pega


    //private void ResetButtonMode()
    //{
    //    unit = null;
    //    builder = null;
    //    spawner = null;
    //}

    private void ResetButton()
    {
        spawner = null;
        spawnerSpawnUnit = null;
    }
    public void SetUpButtonForSpawner(Spawner spawner, AIUnitData vinculationData, Sprite relationedSprite)
    {
        ResetButton();

        this.spawner = spawner;
        spawnerSpawnUnit = vinculationData;
        image.sprite = relationedSprite;        
    }

    //el sistema no es bueno.
    //voy a revisar como es el de sc2 y tratar de copiarle :v
    private void OnButtonPressed()
    {
        switch (currentButtonMode)
        {
            case UIMode.UNIT:
                Debug.LogWarning("no esta implementado!");
                break;

            case UIMode.SPAWNER:
                if (spawner.OnReinforcementMode) { }
                
                break;

            case UIMode.BUILDER:
                Debug.LogWarning("no esta implementado!");
                break;

            case UIMode.OTHER:
                Debug.LogWarning("no esta implementado!");
                break;

            default:
                Debug.LogWarning("no esta implementado!");
                break;
        }
    }

    private void SpawnerSelectionViewPress()
    {

    }
}
