﻿using UnityEngine;
using Items;

public class EntityAnimator : MonoBehaviour
{
    public Animator animator;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void OnPrimary()
    {
        animator.SetTrigger("Primary");
    }

    public virtual void OnSecondary()
    {
        animator.SetTrigger("Secondary");
    }

    public virtual void OnPunchHit()
    {
        animator.SetTrigger("PunchHit");
    }

    public virtual void OnDeath()
    {
        animator.SetTrigger("Death");
    }
    
    public void SetEquipment(Equipment item)
    {
        int currentType = animator.GetInteger("Item");
        int type = (int)item.type;

        if (currentType == type) return;
        if (item.IsMeleeWeapon) 
        {
            animator.SetFloat("MeleeIdle", type);
        }

        animator.SetInteger("Item", type);
    }

    public void SetEquipmentAnimation(EquipmentAnimation animation)
    {
        if (animator.GetInteger("ItemAnimation") == animation.index) return;
        animator.SetInteger("ItemAnimation", animation.index);
    }
}