using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lean.Pool;

public class Health : MonoBehaviour
{    
    public HealthData data;

    [SerializeField]
    private int currentHealth;


    //luego debo implementar para poder recuperar vida
    public void RecieveDamage(int damage)
    {     
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
            //gameObject.SetActive(false);
            LeanPool.Despawn(this);
        }
    }
    
    private void Awake()
    {        
        StartUpVariables();
    }
    private void StartUpVariables()
    {
        currentHealth = data.maxHealth;
    }
}