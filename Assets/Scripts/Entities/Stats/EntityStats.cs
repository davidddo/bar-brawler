﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class EntityStats : MonoBehaviour
{

    public float maxHealth;
    public Stat damage;

    public float CurrentHealth { get; protected set; }
    public event Action OnHealthIsZero;

    [Header("HealthBar")]
    public Image healthBar;


    public virtual void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public virtual void Start()
    {  
    }

    public void TakeDamage(float damage)
    {
        damage = Mathf.Clamp(damage, 0, float.MaxValue);
        CurrentHealth -= damage;
        healthBar.fillAmount = CurrentHealth / maxHealth;
        if (IsDead)
        {
            OnHealthIsZero?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
    }

    public bool IsDead
    {
        get { return CurrentHealth <= 0; }
    }

}
