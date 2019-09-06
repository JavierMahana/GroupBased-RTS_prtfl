using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public event Action<ISelectable> SelectedHaveBeenUpdated = delegate { };
    public event Action<ISelectable> NewSelection = delegate { };

    private const float RAY_LENGTH = 20f;

    public bool canSelect = true;

    public PlayerStateData playerState;

    public ISelectable selected;
    public LayerMask selectableMask;
    
    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && canSelect && !EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("seleccionando");
            DeselectCurrentSelected();
            if (TryToSelect(out ISelectable newSelected))
            {
                SelectNewEntity(newSelected);
            }
            NewSelection(selected);
        }
    }
    private void OnDisable()
    {
        if(!quit)
            selected.SelectionStateChanged -= CallSelectionUpdateEvent;
    }
    private bool quit;
    private void OnApplicationQuit()
    {
        quit = true;
    }



    private void DeselectCurrentSelected()
    {
        if (selected != null)
        {
            selected.SelectionStateChanged -= CallSelectionUpdateEvent;
            selected.Deselect(this);
            selected = null;
        }
    }
    private void CallSelectionUpdateEvent()
    {
        SelectedHaveBeenUpdated(selected);
    }
    private void OnSelectedDeath(IKillable deathSelected)
    {
        deathSelected.OnDeath -= OnSelectedDeath; 

        DeselectCurrentSelected();        
        SelectedHaveBeenUpdated(selected);
    }
    private void SelectNewEntity(ISelectable newSelected)
    {
        selected = newSelected;
        if (selected is IKillable)
        {
            IKillable kSelected = (IKillable)selected;
            kSelected.OnDeath += OnSelectedDeath;
        }

        selected.SelectionStateChanged += CallSelectionUpdateEvent;
        selected.Select(this);       
    }


    private bool TryToSelect(out ISelectable newSelected)
    {        
        Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, CameraController.CAMERA_TO_WORLD_DISTANCE));
        RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero, RAY_LENGTH, selectableMask);

        if (hits == null || hits.Length == 0)
        {
            newSelected = null;
            return false;
        }
        else
        {
            List<ISelectable> selectables = new List<ISelectable>();
            foreach (RaycastHit2D hit in hits)
            {
                ITriggerSelection trigger = hit.transform.GetComponentInParent<ITriggerSelection>();
                
                if (trigger != null)
                {
                    ISelectable current = trigger.GetSelectable();
                    selectables.Add(current);
                }                
            }

            if (selectables == null || selectables.Count == 0)
            {
                newSelected = null;
                return false;
            }
            else
            {
                newSelected = GetBestSelectable(selectables);
                return true;
            }
        }
    }
    private ISelectable GetBestSelectable(IEnumerable<ISelectable> selectables)
    {
        if (GetPlayerTeamSelectables(selectables, out List<ISelectable> playerTeamSelectables))
        {
            return GetSelectableWithMorePriority(playerTeamSelectables);
        }
        else
        {
            return GetSelectableWithMorePriority(selectables);
        }
    }
    private bool GetPlayerTeamSelectables(IEnumerable<ISelectable> selectables, out List<ISelectable> playerTeamSelectables)
    {
        playerTeamSelectables = new List<ISelectable>();
        foreach (ISelectable selectable in selectables)
        {
            if (selectable.Team == playerState.playerTeam)
            {
                playerTeamSelectables.Add(selectable);
            }
        }
        if (playerTeamSelectables.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private ISelectable GetSelectableWithMorePriority(IEnumerable<ISelectable> selectables)    
    {        
        bool unitFound = false;
        bool structureFound = false;
        ISelectable prioritySelectable = null;
        foreach (ISelectable selectable in selectables)
        {
            if (selectable is AIAgent)
            {
                prioritySelectable = selectable;
                break;
            }
            else if (selectable is AIUnit && !unitFound)
            {
                prioritySelectable = selectable;
                unitFound = true;
            }
            else if (selectable is Structure && !structureFound && !unitFound)
            {
                prioritySelectable = selectable;
                structureFound = true;
            }
            else if (!unitFound && !structureFound)
            {
                prioritySelectable = selectable;
            }
        }
        return prioritySelectable;
    }
}
