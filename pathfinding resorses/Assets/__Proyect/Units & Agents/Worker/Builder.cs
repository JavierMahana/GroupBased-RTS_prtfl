using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : AIUnit
{
    [SerializeField]
    [Required]
    private BuilderData data = null;

    public override AIUnitData Data => data;
    public override UIMode UIMode => UIMode.BUILDER;
}
