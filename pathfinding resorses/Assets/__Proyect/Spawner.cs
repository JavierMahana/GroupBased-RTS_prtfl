using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;
using Sirenix.OdinInspector;

public class Spawner : Structure
{        
    public UnitDataToUnit unitDictionary;
    public List<AIUnitData> spawnUnits;
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

            return (Vector2)transform.position + (direction * (Radious + spawnOffSet));
        }
    }

    public override UIMode UIMode { get { return UIMode.SPAWNER; } }
    public override void Select(SelectionManager manager)
    {
        base.Select(manager);
    }
    public override void Deselect(SelectionManager manager)
    {
        base.Deselect(manager);
    }

    public bool TrySpawnUnit(AIUnitData unitData, out AIUnit newUnit)
    {
        AIUnit unitPrefab = unitDictionary.dictionary[unitData];
        Debug.Assert(unitPrefab != null, "the key doen't have any value");

        newUnit = LeanPool.Spawn(unitPrefab, spawningPosition, Quaternion.identity);
        newUnit.team = team;

        return true;
    }
    public bool TrySpawnAgent(AIUnit unit, out AIAgent newAgent)
    {
        AIAgent agentPrefab = unit.data.childPrefab;
        Debug.Assert(agentPrefab != null, "unit data doesn't have a agent prefab");

        if (unit.children.Count >= (int)unit.data.maxSize)
        {
            newAgent = null;
            return false;
        }
        else
        {
            newAgent = LeanPool.Spawn(agentPrefab, spawningPosition, Quaternion.identity);
            unit.AddNewChild(newAgent);

            return true;
        }
    }
}
public enum Direction
{
    RIGHT, UP, LEFT, DOWN
}

