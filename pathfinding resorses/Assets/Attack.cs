using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIAgent))]
public class Attack : EntityAction
{

    // quizas al principio sea más facil hacerlo todo en una clase, pero nose....
    // o mejor hace un sistema de eventos para comunicarse entre componentes.
    
    public AttackData attackData;
    
    protected override void ExecuteAction(IEntity target)
    {
        base.ExecuteAction(target);
        target.RecieveDamage(attackData.attackStrength);
    }
}
