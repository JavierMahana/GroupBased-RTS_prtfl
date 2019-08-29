using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//La funcion de esta clase es manejar el panel flexible.

//Manejar el panel: la idea es que el manager de ordenes acá al panel y este responde, aplicando la info que este tiene.
//así el manager no sabe los detalles, solo sabe lo que quiere. No como se hace.
public class UIFlexibleGridPanel : MonoBehaviour
{
    public const int FLEXIBLE_PANEL_SPACES = 8;
    public const int PAGE_BUTTONS_COUNT = 2;

    public RectTransform[] panelSpacePlaceHolders;
    public UIPageButton[] pageButtons;


    private void Asserts()
    {
        Debug.Assert(panelSpacePlaceHolders != null && panelSpacePlaceHolders.Length == FLEXIBLE_PANEL_SPACES, $"set up the {FLEXIBLE_PANEL_SPACES} buttonPlaceHolders!");
        foreach (var placeholder in panelSpacePlaceHolders) { Debug.Assert(placeholder != null, "All the place holder slots must have a values!"); }

        Debug.Assert(pageButtons != null && pageButtons.Length == PAGE_BUTTONS_COUNT, $"set up the {PAGE_BUTTONS_COUNT} page button!");
        foreach (UIPageButton button in pageButtons) { Debug.Assert(button != null, "All the page buttons must have a values!"); }
    }
}
