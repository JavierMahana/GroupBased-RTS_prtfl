using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack : BaseAction
{
    public override BaseActionData BaseData { get { return attackData; } }
    public AttackData attackData;
    protected override void ExecuteAction(ITargetable target)
    {
        base.ExecuteAction(target);

        if (target == null) return;        
        target.RecieveDamage(attackData.attackStrength);
    }
}
