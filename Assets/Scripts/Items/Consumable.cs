﻿using System.Collections;
using UnityEngine;
using Utils;

namespace Items
{
    public class Consumable : Equippable
    {
        private float newHealth;
        private float healingSpeed;
        private bool drunk = false;

        void Update()
        {
            if (drunk)
            {
                if (owner.stats.CurrentHealth < newHealth)
                {
                    owner.stats.Heal(healingSpeed * Time.deltaTime);
                }
            }
        }

        public override void OnPrimary()
        {
            base.OnPrimary();
            if (owner.stats == null || owner.stats.HasFullLife || owner.combat.IsAttacking || owner.combat.IsBlocking || owner.combat.IsDrinking) return;
            owner.animator.OnPrimary();
        }

        public virtual void StartDrinking()
        {
            if ((item as Drink).healingDelay > 0)
            {
                StartCoroutine(DelayedHealing((item as Drink).healingDelay));
                return;
            }

            StartHealing();
        }

        private IEnumerator DelayedHealing(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartHealing();
        }

        private void StartHealing()
        {
            newHealth = owner.stats.CurrentHealth + (item as Drink).healingAmount;
            healingSpeed = (item as Drink).healingSpeed;
            drunk = true;
        }

        public IEnumerator DestroyAfterTime(float time, FunctionUpdater updater = null)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);

            if (updater != null) updater.DestroySelf();
        }
    }
}
