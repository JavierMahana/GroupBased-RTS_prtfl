using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Entity))]
public class Spawner : MonoBehaviour
{    
    public static event Action<Spawner> OnSelect  = delegate{ };

    public List<AIUnitData> unitsToSpawn;
    public Direction spawnDirection;
    public float spawnOffSet = 0;
    private Vector2 spawningPosition
    {
        get
        {
            Vector2 direction = Vector2.zero;
            switch (spawnDirection)
            {
                case Direction.RIGHT:
                    direction = Vector2.right;
                    break;
                case Direction.UP:
                    direction = Vector2.up;
                    break;
                case Direction.LEFT:
                    direction = Vector2.left;
                    break;
                case Direction.DOWN:
                    direction = Vector2.down;
                    break;
            }

            return (Vector2)transform.position + (direction * (entity.Radious + spawnOffSet));
        }
    }         
    private Team team { get { return entity.Team; } }
    private Entity entity;



    [Button]
    private void Select()
    {
        OnSelect(this);
    }

    public AIUnit SpawnUnit(AIUnitData unitData)
    {
        AIUnit unitPrefab = UnitManager.Instance.unitDictionary.dictionary[unitData];
        Debug.Assert(unitPrefab != null, "the key doen't have any value");
        AIUnit spawnedUnit = LeanPool.Spawn(unitPrefab, spawningPosition, Quaternion.identity);
        spawnedUnit.team = team;

        return spawnedUnit;
    }
    public AIAgent SpawnAgent(AIUnit unit)
    {
        AIAgent agentPrefab = unit.data.childPrefab;
        Debug.Assert(agentPrefab != null, "unit data doesn't have a agent prefab");

        AIAgent spawnedAgent = LeanPool.Spawn(agentPrefab, spawningPosition, Quaternion.identity);
        unit.AddNewChild(spawnedAgent);

        return spawnedAgent;
    }

    private void Start()
    {
        entity = GetComponent<Entity>();        
    }
}
public enum Direction
{
    RIGHT, UP, LEFT, DOWN
}

