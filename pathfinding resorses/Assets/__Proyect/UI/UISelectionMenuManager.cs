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
    public UnitDataToSprite portraitDictionary;
    public UIStaticGridPanel staticGridPanel;
    public UIFlexibleGridPanel flexibleGridPanel;

    public SelectionManager selectionManager;

    private void Awake()
    {
        InitAsserts();
        selectionManager.NewSelection += ShowNewSelectionUI;
        selectionManager.SelectedHaveBeenUpdated += UpdateSelectionUI;
    }
    private void InitAsserts()
    {
        Debug.Assert(staticGridPanel != null);
        Debug.Assert(flexibleGridPanel != null);
        Debug.Assert(portraitDictionary != null, "assign the portrait dictionary!");
        //Debug.Assert(spawnerUI != null);
        //Debug.Assert(unitUI != null);
        Debug.Assert(selectionManager != null);
    }

    private void ShowNoSelectionView()
    {
        staticGridPanel.NoSelected();
        flexibleGridPanel.NoSelected();
    }


    //llamado cuando se ocurre una accion de nueva selección.
    public void ShowNewSelectionUI(ISelectable selectable)
    {
        if (selectable == null)
        {
            ShowNoSelectionView();
        }
        else
        {
            switch (selectable.UIMode)
            {
                case UIMode.UNIT:
                    Debug.Assert(selectable is AIUnit, "check the type of the selectable");
                    AIUnit unit = (AIUnit)selectable;
                    staticGridPanel.NewSelection(unit,this);
                    flexibleGridPanel.NewSelection(unit);

                    break;


                case UIMode.SPAWNER:
                    Debug.Assert(selectable is Spawner, "check the type of the selectable");
                    Spawner spawner = (Spawner)selectable;
                    staticGridPanel.NewSelection(spawner, this);
                    flexibleGridPanel.NewSelection(spawner);

                    break;

                case UIMode.BUILDER:
                    Debug.Assert(selectable is Builder, "check the type of the selectable");
                    Builder builder = (Builder)selectable;
                    staticGridPanel.NewSelection(builder, this);
                    flexibleGridPanel.NewSelection(builder);

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
    //llamado cuando la selección actual cambia de estado interno.
    public void UpdateSelectionUI(ISelectable selectable)
    {        
        if (selectable == null)
        {
            ShowNoSelectionView();
        }
        else
        {
            switch (selectable.UIMode)
            {
                case UIMode.UNIT:
                    Debug.Assert(selectable is AIUnit, "check the type of the selectable");
                    AIUnit unit = (AIUnit)selectable;
                    staticGridPanel.UpdateSelection(unit, this);
                    flexibleGridPanel.UpdateSelection(unit);

                    break;


                case UIMode.SPAWNER:
                    Debug.Assert(selectable is Spawner, "check the type of the selectable");
                    Spawner spawner = (Spawner)selectable;
                    staticGridPanel.UpdateSelection(spawner, this);
                    flexibleGridPanel.UpdateSelection(spawner);

                    break;

                case UIMode.BUILDER:
                    Debug.Assert(selectable is Builder, "check the type of the selectable");
                    Builder builder = (Builder)selectable;
                    staticGridPanel.UpdateSelection(builder, this);
                    flexibleGridPanel.UpdateSelection(builder);

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
