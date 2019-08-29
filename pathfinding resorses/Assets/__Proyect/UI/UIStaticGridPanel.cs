using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStaticGridPanel : MonoBehaviour
{
    public const int STATIC_PANEL_SPACES = 9;
    public RectTransform[] panelSpacePlaceHolders;


    private void Asserts()
    {
        Debug.Assert(panelSpacePlaceHolders != null && panelSpacePlaceHolders.Length == STATIC_PANEL_SPACES, $"set up the {STATIC_PANEL_SPACES} buttonPlaceHolders!");
        foreach (var placeholder in panelSpacePlaceHolders) { Debug.Assert(placeholder != null, "All the place holder slots must have a values!"); }
    }
}
