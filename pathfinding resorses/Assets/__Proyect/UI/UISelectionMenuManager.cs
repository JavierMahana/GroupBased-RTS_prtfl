using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;
using Sirenix.OdinInspector;

public enum UIMode
{
    UNIT,
    SPAWNER,
    BUILDER,
    OTHER
}
//ahora mismo solo permite estructuras spawneadoras.
//la idea es que luego se permitan unidades spawneadoras (?
public class UISelectionMenuManager : MonoBehaviour
{
    //Quizas sea bueno tener un manager de los paneles

    public UnitDataToSprite portraitDictionary;
    public UIStaticGridPanel staticGridPanel;
    public UIFlexibleGridPanel flexibleGridPanel;


    public string deselectionEvent;

    public SelectionManager selectionManager;

    public UISpawnerManager spawnerUI;
    public UIUnitManager unitUI;
    
    private void Awake()
    {
        InitAsserts();
        selectionManager.SelectedHaveBeenUpdated += UpdateSelectionUI;
    }
    private void InitAsserts()
    {
        Debug.Assert(staticGridPanel != null);
        Debug.Assert(flexibleGridPanel != null);
        Debug.Assert(portraitDictionary != null, "assign the portrait dictionary!");
        Debug.Assert(spawnerUI != null);
        Debug.Assert(unitUI != null);
        Debug.Assert(selectionManager != null);
    }

    /// <summary>
    /// Devuelve el valor de elementos que se deben mostrar en el panel flexible, tomando en cuenta la pagina
    /// </summary>
    /// <param name="totalElements">cantidad total de elementos. Por ejemplo el numero de hijos de una unidad a mostrar</param>
    public int GetFlexiblePanelCount(int totalElements, int page)
    {
        //count es la cantidad de paneles que se van a mostrar en esta pagina.
        //si el valor maximo, relacionado con la pagina actual, es menor a la cuenta de niños. la cuenta debe ser el PS  
        int count;
        int pageMaxCount = FLEXIBLE_PANEL_SPACES * page;
        if (pageMaxCount <= totalElements)
        {
            count = FLEXIBLE_PANEL_SPACES;
        }
        //si no, el count debe ser el modulo (se elimina la situacion de que de modulo sea 0, con la condición anterior)
        else
        {
            int countMod = totalElements % FLEXIBLE_PANEL_SPACES;
            count = countMod;
        }
        return count;
    }
    public void UpdateSelectionUI(ISelectable selectable)
    {        
        if (selectable == null)
        {
            GameEventMessage.SendEvent(deselectionEvent);
        }
        else
        {
            switch (selectable.UIMode)
            {
                case UIMode.UNIT:
                    Debug.Assert(selectable is AIUnit, "check the type of the selectable");
                    AIUnit unit = (AIUnit)selectable;
                    unitUI.ShowUnitSelection(unit);

                    break;


                case UIMode.SPAWNER:
                    Debug.Assert(selectable is Spawner, "check the type of the selectable");
                    Spawner spawner = (Spawner)selectable;

                    if (spawner.OnReinforcementMode) spawnerUI.ShowReinforcementView(spawner);
                    else spawnerUI.ShowCreationView(spawner);

                    break;

                case UIMode.BUILDER:
                    Debug.Assert(selectable is Builder, "check the type of the selectable");
                    Builder builder = (Builder)selectable;


                    break;

                case UIMode.OTHER:
                    Debug.LogWarning("no esta implementado!");
                    break;
                default:
                    Debug.LogWarning("no esta implementado!");
                    break;
            }
        }        
    }    
}
