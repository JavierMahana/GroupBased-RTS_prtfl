using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Shape
{
    SQUARE,
    CIRCULAR
}
public interface IEntity
{
    float Radious { get; }
    Shape BodyShape { get; } 
    GameObject GameObject { get; }
    Team Team { get; }
    void RecieveDamage(int attackStrenght);
}
