using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
