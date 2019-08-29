using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using System;
using Sirenix.OdinInspector;

public class Spawner : Structure, ISelectable
{
    public event SelectionEvent SelectionStateChanged = delegate { };


    public UnitDataToUnit unitDictionary;
    public List<AIUnitData> spawnUnits;
    public Direction spawnDirection;
    public float spawnOffSet = 0;
    public UIMode UIMode { get { return UIMode.SPAWNER; } }
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


    //volatile variables
    public bool OnReinforcementMode { get { return currentUnitData != null; } }
    public List<AIUnit> currentReinforcementCompatibleUnits { get; private set; }
    public AIUnitData currentUnitData { get; private set; }

    

    private void UnitSpawned(AIUnit unit)
    {
        if (! OnReinforcementMode) return;

        if (this.Team == unit.Team && this.currentUnitData == unit.Data)
        {
            currentReinforcementCompatibleUnits.Add(unit);
            unit.OnDeath += OnReinforcementUnitDeath;

            SelectionStateChanged();
        }
    }    
    private void OnReinforcementUnitDeath(IKillable unit)
    {
        Debug.Assert(unit is AIUnit);
        SelectionStateChanged();
    }

    public void SetReinforcementMode(AIUnitData data, UnitManager unitManager)
    {
        ResetSpawner();

        currentUnitData = data;
        currentReinforcementCompatibleUnits = unitManager.FindUnits(data, Team);
        foreach (IKillable unit in currentReinforcementCompatibleUnits)
        {
            unit.OnDeath += OnReinforcementUnitDeath;
        }
    }
    public void ResetSpawner()
    {
        if (!OnReinforcementMode) return;

        foreach (IKillable unit in currentReinforcementCompatibleUnits)
        {
            unit.OnDeath -= OnReinforcementUnitDeath;
        }
        currentReinforcementCompatibleUnits = null;
        currentUnitData = null;
    }




    public void Select(SelectionManager manager)
    {
        
    }
    public void Deselect(SelectionManager manager)
    {
        ResetSpawner();
    }
    public ISelectable GetSelectable()
    {
        return this;
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
        AIAgent agentPrefab = unit.Data.childPrefab;
        Debug.Assert(agentPrefab != null, "unit data doesn't have a agent prefab");

        if (unit.children.Count >= (int)unit.Data.maxSize)
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
    protected override void OnEnable()
    {
        base.OnEnable();
        AIUnit.OnSpawn += UnitSpawned;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        AIUnit.OnSpawn -= UnitSpawned;
    }

}
public enum Direction
{
    RIGHT, UP, LEFT, DOWN
}

