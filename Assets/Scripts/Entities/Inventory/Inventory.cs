﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate void OnItemChanged();
    public OnItemChanged onItemChanged;

    public List<Item> items = new List<Item>();
    public Item currentSelected;
    public int maxItems = 10;


    public bool AddItem(Item item) {
        if (item.addToInventory)
        {
            if (items.Count >= maxItems) return false;

            items.Add(item);
            if (onItemChanged != null) onItemChanged.Invoke();

            return true;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        if (onItemChanged != null) onItemChanged.Invoke();
    }
}
