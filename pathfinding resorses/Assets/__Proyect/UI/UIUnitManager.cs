using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Lean.Pool;
using Doozy.Engine;

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
        RectTransform[] gridPlaceHolders = selectionMenuManager.selectionGridPlaceHolders;
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
    public void ShowUnitSelection(AIUnit unit)
    {
        GameEventMessage.SendEvent(showUnitUIEvent);

        List<AIAgent> unitChildren = unit.children;
        int count = unitChildren.Count;

        UpdateAgentPanels(unit, count, unitChildren);
        HideAgentPanels();
        ShowAgentPanels(count);
    }

    private void UpdateAgentPanels(AIUnit unit, int count, List<AIAgent> unitChildren)
    {
        for (int i = 0; i < count; i++)
        {
            AIAgent child = unitChildren[i];

            Debug.Assert(selectionMenuManager.portraitDictionary.dictionary.TryGetValue(child.parent.data, out Sprite sprite), "update the portrait dictionary!");

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
