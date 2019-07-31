using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    Entity entity;
    
    public HealthData data;
    public Action InvokeDeath = delegate { };


    [HideInInspector]
    private int currentHealth;


    //luego debo implementar para poder recuperar vida
    public void RecieveDamage(int damage)
    {
     


        //Debug.Log($"ayuda :{agent}");
        int trueDamage = damage - data.defense;
        if (trueDamage < 1)
        {
            trueDamage = 1;
        }
        currentHealth -= trueDamage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentHealth <= 0)
        {
            InvokeDeath();
        }
    }
    
    private void Awake()
    {
        entity = GetComponent<Entity>();
        if (entity == null || data == null) Debug.LogError("Must assign variables!");
        
        StartUpVariables();
    }
    private void StartUpVariables()
    {
        currentHealth = data.maxHealth;
    }
}