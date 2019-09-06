using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplayable
{
    string DisplayName { get; }
    Sprite DisplayIcon { get; }
    string DisplayDesc { get; }
    Health DisplayHealth { get; }
    Vector2Int? CapacityDisplay { get; } 
    List<IDisplayable> DependentDisplayables { get; }
    List<AbilityData> Abilities { get; }    
    ComandCardButton[,] CommandCardButtons { get; }
    Dictionary<BaseButtonData, BaseCommand> ButtonToCommandDictionary { get; }
}
