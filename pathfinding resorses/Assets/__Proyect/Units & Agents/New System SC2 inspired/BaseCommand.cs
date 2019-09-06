using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCommand : ScriptableObject
{    
    public virtual void Execute(IDisplayable entity)
    {
    }
}
