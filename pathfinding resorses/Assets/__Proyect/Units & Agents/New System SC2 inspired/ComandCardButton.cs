using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ComandType
{
    ABILITY,
    SUBMENU,
    CLOSE_SUBMENU
}
public class ComandCardButton : SerializedScriptableObject
{
    //debe hacer asserts y weas dsps
    public ComandType type = ComandType.ABILITY;
    public BaseButtonData button = null;

    [ShowIf("onAbilityComand")]
    public AbilityData ability = null;


    public void AddButtonCommandLink(IDisplayable entity)
    {
        entity.ButtonToCommandDictionary.Add(button, ability.baseCommand);
    }

    #region used for attributes


    private bool onAbilityComand => type == ComandType.ABILITY;
    #endregion
}
