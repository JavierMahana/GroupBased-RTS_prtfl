using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActable 
{
    event ActionEvent OnActionStart;
}
public delegate void ActionEvent(ITargetable target);
