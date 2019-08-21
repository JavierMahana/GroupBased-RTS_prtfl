﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//los agentes referencian a los a datos de su padre
public interface ISelectable : ITriggerSelection
{    
    event SelectionEvent SelectionStateChanged;
    void Select(SelectionManager manager);
    void Deselect(SelectionManager manager);    
    Team Team { get; }
    UIMode UIMode { get; }
}
public delegate void SelectionEvent();
public enum UIMode
{
    UNIT,
    SPAWNER,
    OTHER
}