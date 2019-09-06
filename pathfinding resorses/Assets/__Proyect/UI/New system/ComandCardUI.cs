using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Lean.Pool;


//hace asserts despues
//este es un manager enverdad
public class ComandCardUI : SerializedMonoBehaviour
{
    public const int ROW_COUNT = 3;
    public const int COLUMN_COUNT = 3;

    public ButtonDisplay buttonDisplayPrefab;

    //index:
    // 0 | 1 | 2
    // 3 | 4 | 5
    // 6 | 7 | 8
    
    public RectTransform[] cardSpaces = new RectTransform[ROW_COUNT * COLUMN_COUNT];
    private ButtonDisplay[] buttonInstances = new ButtonDisplay[ROW_COUNT * COLUMN_COUNT];
    private void Awake()
    {
        Init();
    }

    public void Prime(IDisplayable displayable)
    {
        Refresh();

        ComandCardButton[,] comButtons = displayable.CommandCardButtons;
        for (int y = 0; y < comButtons.GetLength(1); y++)
        {
            for (int x = 0; x < comButtons.GetLength(0); x++)
            {
                if (comButtons[x, y] = null) continue;

                buttonInstances[x + y * comButtons.GetLength(0)].Prime(comButtons[x, y].button);
            }
        }
    }



    private void Refresh()
    {
        throw new System.NotImplementedException();
        //refresca los botones, para que así se puedan colocar nuevos valores
        //los desactiva a todos
    }
    private void Init()
    {
        for (int i = 0; i < cardSpaces.Length; i++)
        {
            RectTransform space = cardSpaces[i];

            ButtonDisplay currButton = LeanPool.Spawn(buttonDisplayPrefab);
            currButton.transform.SetParent(space, false);
            RectTransform rTransform = (RectTransform)currButton.transform;
            rTransform.anchorMin = new Vector2(0, 0);
            rTransform.anchorMax = new Vector2(1, 1);
            rTransform.offsetMin = Vector2.zero;
            rTransform.offsetMax = Vector2.zero;

            buttonInstances[i] = currButton;
        }
    }
}
