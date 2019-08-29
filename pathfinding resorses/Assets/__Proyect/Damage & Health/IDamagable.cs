using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable  
{
    Health Health { get; }
    void RecieveDamage(int attackStrenght);
}
