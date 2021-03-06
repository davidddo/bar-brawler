﻿using UnityEngine;
using Items;

public class EntityCombatBehaviour : StateMachineBehaviour
{
   public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Entity entity = animator.GetComponentInParent<Entity>();
        if (entity != null && entity.equipment.CurrentEquipment != null && !entity.combat.IsBlocking)
        {
            entity.combat.SetState(entity.equipment.CurrentEquipment);

            if (entity is Player)
            {
                Player player = entity as Player;
                Equipment item = player.equipment.CurrentEquipment;
                if (item != null && item.IsDrink)
                {
                    //AudioManager.instance.PlaySound(Sound.Drinking);
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Entity entity = animator.GetComponentInParent<Entity>();
        if (entity != null && entity.equipment.CurrentEquipment != null)
        {
            entity.combat.SetState(CombatState.Idle);

            if (entity is Player)
            {
                Player player = entity as Player;
                Equipment item = player.equipment.CurrentEquipment;
                if (item != null && item.IsDrink)
                {
                    Player.instance.inventory.RemoveItem(item);
                }
            }
        }
    }
}
