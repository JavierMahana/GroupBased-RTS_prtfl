using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    //va a tener valores desde la agent data por ahora.
    // luego la idea es que haya información especificamente para esto.
    AIAgent agent;
    AIAgentData data;
    public Action InvokeDeath = delegate { };


    [HideInInspector]
    public int currentHealth;


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
        if (currentHealth == 0)
        {
            InvokeDeath();
        }
    }
    
    private void Awake()
    {
        agent = GetComponent<AIAgent>();
        data = agent.data;
        StartUpVariables();
    }
    private void StartUpVariables()
    {
        currentHealth = data.maxHealth;
    }
}
