﻿using Items;

public class PlayerEquipment : EntityEquipment
{
    private Inventory inventory;
    private Hotbar hotbar;

    protected override void Start()
    {
        base.Start();

        inventory = Player.instance.inventory;
        inventory.OnItemRemoved += OnItemRemoved;

        hotbar = FindObjectOfType<Hotbar>();
        hotbar.OnItemSelected += EquipItem;

        EquipItem(inventory.defaultItems[0] as Equipment);
    }

    private void OnItemRemoved(object sender, InventoryEvent e)
    {
        if (e.item == currentEquipment)
        {
            Unequip();
        }
    }
}