using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

//las habilidades y/o las disployables, se subscriben al evento "Click"
//requerimientos para mostrarse deben estar acá
public class BaseButtonData : ScriptableObject
{
    public string buttonName;
    public Sprite iconSprite;
    public void Invoke()
    {
        OnClick();
    }

    public event Action OnClick = delegate { };
}
