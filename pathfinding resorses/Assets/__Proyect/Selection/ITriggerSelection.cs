using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers selection by his collider. Used in selection manager.
/// </summary>
public interface ITriggerSelection
{
    ISelectable GetSelectable();
}
