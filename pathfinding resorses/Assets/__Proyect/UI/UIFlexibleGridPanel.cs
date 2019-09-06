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

    public void NoSelected()
    {

    }

    public void NewSelection(AIUnit selected)
    {
    }
    public void NewSelection(Spawner selected)
    {
    }
    public void NewSelection(Builder selected)
    {
    }

    public void UpdateSelection(AIUnit selected)
    {

    }
    public void UpdateSelection(Spawner selected)
    {

    }
    public void UpdateSelection(Builder selected)
    {

    }

    private void Asserts()
    {
        Debug.Assert(panelSpacePlaceHolders != null && panelSpacePlaceHolders.Length == FLEXIBLE_PANEL_SPACES, $"set up the {FLEXIBLE_PANEL_SPACES} buttonPlaceHolders!");
        foreach (var placeholder in panelSpacePlaceHolders) { Debug.Assert(placeholder != null, "All the place holder slots must have a values!"); }

        Debug.Assert(pageButtons != null && pageButtons.Length == PAGE_BUTTONS_COUNT, $"set up the {PAGE_BUTTONS_COUNT} page button!");
        foreach (UIPageButton button in pageButtons) { Debug.Assert(button != null, "All the page buttons must have a values!"); }
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
}
