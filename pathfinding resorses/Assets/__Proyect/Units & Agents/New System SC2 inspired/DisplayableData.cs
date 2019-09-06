using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

//es medio enredado actualmente
//separar un poco las cosas dsps
[CreateAssetMenu(menuName ="serializable test")]
public class DisplayableData : SerializedScriptableObject
{
    public string displayName;
    public Sprite icon;
    public string desc;        
    [TableMatrix(HideColumnIndices = true, HideRowIndices = true, HorizontalTitle ="Comand Card Buttons")]
    public BaseButtonData[,] comandCardButtons = new BaseButtonData[ComandCardUI.ROW_COUNT, ComandCardUI.COLUMN_COUNT];
}
public enum DisplayableFlags
{

}
