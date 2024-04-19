using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CreatureStats
{
    public float MaxHP;
    public float CurrentHP;
    public float Damage;
    public event Action Die;
    public CreatureStats(float maxHP, float currentHP, float damage)
    {
        MaxHP = maxHP;
        CurrentHP = currentHP;
        Damage = damage;
        Die = null;
    }
    public void GetDamage(float damage)
    {
        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die?.Invoke();
        }
    }
}
