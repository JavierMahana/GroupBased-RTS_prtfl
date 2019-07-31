using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public abstract class Entity : SerializedMonoBehaviour, IEntity
{    
    [HideInInspector]
    public Action<Entity> OnEntityDeath = delegate { };
    [HideInInspector]
    public Action<Entity> ActionBegan = delegate { };

    public abstract float Radious { get; }
    public abstract Shape BodyShape { get; }
    public abstract GameObject GameObject { get; }
    public abstract Team Team { get; }
    public abstract void RecieveDamage(int attackStrenght);
}
