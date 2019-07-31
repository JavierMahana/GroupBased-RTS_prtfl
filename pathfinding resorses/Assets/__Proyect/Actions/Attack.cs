using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIAgent))]
public class Attack : EntityAction
{
    public override BaseActionData BaseData { get { return attackData; } }
    public AttackData attackData;
    protected override void ExecuteAction(IEntity target)
    {
        base.ExecuteAction(target);

        if (target == null) return;
        //Debug.Log($"piña va! {agent} a {agent.Target}");
        target.RecieveDamage(attackData.attackStrength);
    }
}
