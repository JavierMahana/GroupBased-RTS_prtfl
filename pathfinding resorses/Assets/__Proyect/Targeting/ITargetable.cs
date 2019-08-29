using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable : IKillable, IDamagable
{     
    float Radious { get; }
    GameObject GameObject { get; }
    Team Team { get; }
    Shape BodyShape { get; }
}
public enum Shape
{
    SQUARE,
    CIRCULAR
}
