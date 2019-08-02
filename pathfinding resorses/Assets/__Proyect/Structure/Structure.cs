using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
//not implemented yet
public class Structure : Entity
{

    
    public StructureData data;
    public Team team;
    public Vector2Int gridPosition;
    private Health health;



    public override float Radious { get { return data.radious; } }
    public override Shape BodyShape { get { return data.bodyShape; } }
    public override GameObject GameObject { get { return gameObject; } }
    public override Team Team { get { return team; } }
    public override Health Health { get { return health; } }


    private void Start()
    {
        health = GetComponent<Health>();
    }
}
