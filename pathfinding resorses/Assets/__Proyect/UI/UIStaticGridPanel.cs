using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaticGridPanel : MonoBehaviour
{
    public const int STATIC_PANEL_SPACES = 9;
    public UIStaticGridButton[] gridButtons;

    private void SetUpAndShowGridButtonsForSpawnerSelectionView(Spawner selected, UISelectionMenuManager caller)
    {
        for (int i = 0; i < STATIC_PANEL_SPACES; i++)
        {
            AIUnitData spawnerSpawnOption = selected.spawnOptions[i];
            if (spawnerSpawnOption == null) continue;


            Debug.Assert(caller.portraitDictionary.dictionary.TryGetValue(spawnerSpawnOption, out Sprite sprite),
                $"portrait dictionary doesn't have a value for {spawnerSpawnOption}");

            gridButtons[i].SetUpButtonForSpawner(selected, spawnerSpawnOption, sprite);
            gridButtons[i].gameObject.SetActive(true);
        }
    }
    //private void ResetAllButtons()
    //{
    //    foreach (var button in gridButtons)
    //    {
    //        button.ResetButton();
    //    }
    //}
    private void HideAllContent()
    {
        foreach (var button in gridButtons)
        {
            button.gameObject.SetActive(false);
        }
    }


    public void NoSelected()
    {

    }

    public void NewSelection(AIUnit selected, UISelectionMenuManager caller)
    {
    }
    public void NewSelection(Spawner selected, UISelectionMenuManager caller)
    {
        HideAllContent();
        //ResetAllButtons();
        SetUpAndShowGridButtonsForSpawnerSelectionView(selected, caller);
    }



    public void NewSelection(Builder selected, UISelectionMenuManager caller)
    {
    }


    



    public void UpdateSelection(AIUnit selected, UISelectionMenuManager caller)
    {

    }
    public void UpdateSelection(Spawner selected, UISelectionMenuManager caller)
    {

    }
    public void UpdateSelection(Builder selected, UISelectionMenuManager caller)
    {

    }



    private void Asserts()
    {
        Debug.Assert(gridButtons != null && gridButtons.Length == STATIC_PANEL_SPACES, $"set up the {STATIC_PANEL_SPACES} buttonPlaceHolders!");
        foreach (var button in gridButtons) { Debug.Assert(button != null, "All the place holder slots must have a values!"); }
    }
}
