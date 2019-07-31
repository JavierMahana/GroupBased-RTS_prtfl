using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Entity/Health/Data")]
public class HealthData : ScriptableObject
{
    public int maxHealth = 100;
    public int defense = 1;

}
