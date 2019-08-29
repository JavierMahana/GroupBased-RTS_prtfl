using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Lean.Pool;
using Doozy.Engine;


//los agent panels estarán en el panel flexible y las habilidades en el estatico
[RequireComponent(typeof(UISelectionMenuManager))]
public class UIUnitManager : MonoBehaviour
{
    [Required]
    public RectTransform agentPanelPrefab;
    public string showUnitUIEvent;
    private AgentPanel[] unitAgentPanels;
    private UISelectionMenuManager selectionMenuManager;

    private void Start()
    {
        selectionMenuManager = GetComponent<UISelectionMenuManager>();
        InitAgentPanels();
    }
    private void InitAgentPanels()
    {
        RectTransform[] gridPlaceHolders = selectionMenuManager.staticGridPanelPlaceHolders;
        unitAgentPanels = new AgentPanel[gridPlaceHolders.Length];

        for (int i = 0; i < gridPlaceHolders.Length; i++)
        {

            RectTransform placeHolder = gridPlaceHolders[i];
        
            AgentPanel tempPanel = placeHolder.GetComponentInChildren<AgentPanel>();
            if (tempPanel != null)
            {
                unitAgentPanels[i] = tempPanel;
                continue;
            }

            AgentPanel newPanel;
            RectTransform rectTransform = LeanPool.Spawn(agentPanelPrefab);
            SetRectPosition(placeHolder, rectTransform);

            newPanel = rectTransform.GetComponentInChildren<AgentPanel>();
            Debug.Assert(newPanel != null, "fix the panel prefab!");



            unitAgentPanels[i] = newPanel;
        }
    }
    private void SetRectPosition(Transform parentPlaceHolder, RectTransform rectTransform)
    {
        rectTransform.SetParent(parentPlaceHolder, false);

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = new Vector2(1, 1);

        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }
    public void ShowUnitSelection(AIUnit unit, int page = 1)
    {
        GameEventMessage.SendEvent(showUnitUIEvent);

        List<AIAgent> unitChildren = unit.children;

        int count = selectionMenuManager.GetFlexiblePanelCount(unitChildren.Count, page);        

        UpdateAgentPanels(unit, count, unitChildren, page);
        HideAgentPanels();
        ShowAgentPanels(count);
    }
    //must be on 
    public void ChangePageOfUnitSelection(AIUnit unit, int page)
    {
        List<AIAgent> unitChildren = unit.children;

        int count = selectionMenuManager.GetFlexiblePanelCount(unitChildren.Count, page);

        UpdateAgentPanels(unit, count, unitChildren, page);
        HideAgentPanels();
        ShowAgentPanels(count);
    }


    private void UpdateAgentPanels(AIUnit unit, int count, List<AIAgent> unitChildren, int page)
    {
        for (int i = 0; i < count; i++)
        {
            //el index del hijo es el relacionado al numero de la pagina
            AIAgent child = unitChildren[i + (UISelectionMenuManager.FLEXIBLE_PANEL_SPACES * (page - 1))];

            Debug.Assert(selectionMenuManager.portraitDictionary.dictionary.TryGetValue(child.parent.Data, out Sprite sprite), "update the portrait dictionary!");

            unitAgentPanels[i].UpdatePanel(child, sprite);
        }
    }
    private void ShowAgentPanels(int count)
    {
        for (int i = 0; i < count; i++)
        {
            unitAgentPanels[i].gameObject.SetActive(true);
        }
    }
    private void HideAgentPanels()
    {
        foreach (AgentPanel panel in unitAgentPanels)
        {
            panel.gameObject.SetActive(false);
        }
    }
}
