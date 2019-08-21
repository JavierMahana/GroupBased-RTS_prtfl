using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;
using Sirenix.OdinInspector;


//ahora mismo solo permite estructuras spawneadoras.
//la idea es que luego se permitan unidades spawneadoras (?
public class UISelectionMenuManager : MonoBehaviour
{    
    [Required]
    public UnitDataToSprite portraitDictionary;
    public RectTransform[] selectionGridPlaceHolders;

    public string deselectionEvent;

    public SelectionManager selectionManager;

    public UISpawnerManager spawnerUI;
    public UIUnitManager unitUI;
    
    private void Awake()
    {
        InitVariables();
        selectionManager.SelectedHaveBeenUpdated += UpdateSelectionUI;
    }
    private void InitVariables()
    {
        Debug.Assert(selectionGridPlaceHolders != null && selectionGridPlaceHolders.Length == 9, "set up the 9 buttonPlaceHolders!");
        foreach (var placeholder in selectionGridPlaceHolders) { Debug.Assert(placeholder != null, "All the place holder slots must have a values!"); }
        Debug.Assert(portraitDictionary != null, "assign the portrait dictionary!");
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
                    spawnerUI.ShowCreationView(spawner);
                    break;


                case UIMode.OTHER:
                    Debug.LogWarning("no esta implementado!");
                    break;
                default:
                    break;
            }
        }        
    }    
}
