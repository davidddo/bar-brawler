﻿using System.Collections.Generic;
using UnityEngine;
using Items;

/// <summary>
/// Class <c>Inventory</c> is used to store all the items the player received or
/// rather buyed in the store. The inventory is split up in 5 slots which contains
/// the items. Each slot can hold the set amount of items the specific item declared.
/// With the method <see cref="AddItem(Items.Item)"/> a new item can be added to
/// the inventory and removed with the method <see cref="RemoveItem(Items.Item)"/>.
/// The inventory also keeps track of the current amount of ammunition the player has. 
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    private readonly int maxSlots = 5;

    public List<InventorySlot> Slots { get; protected set; } = new List<InventorySlot>();

    [Header("Munition")]
    public int currentMunition = 0;
    public int maxMunition = 30;

    public delegate void MunitionUpdate(int currentAmount);
    public event MunitionUpdate OnMunitionUpdate;

    public delegate void InventoryUpdate(Item item);


    public event InventoryUpdate OnItemAdded;
    public event InventoryUpdate OnItemRemoved;
    public event InventoryUpdate OnItemUsed;

    void Start()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            Slots.Add(new InventorySlot(i));
        }
    }

    /// <summary>
    /// This method adds a new item to the inventory if it is declared as a
    /// inventory item. If this is the case, a slot with the same item or a free
    /// one is searched for. If a slot is found the given item is added to it and
    /// the event <see cref="OnItemAdded"/> gets fired.
    /// </summary>
    /// <param name="item">The item which should be added to the inventory.</param>
    public void AddItem(Item item)
    {
        if (item == null) return;
        if (item.addToInventory)
        {
            InventorySlot freeSlot = FindStackableSlot(item);
            if (freeSlot == null) freeSlot = FindNextEmptySlot();

            if (freeSlot != null)
            {
                freeSlot.Add(item);
                OnItemAdded?.Invoke(item);
            }
        }
    }

    /// <summary>
    /// This method removes the given item from the inventory if it could be
    /// found in on of the slots and will the fire the <see cref="OnItemRemoved"/>
    /// event.
    /// </summary>
    /// <param name="item">The item which should be removed from the inventory.</param>
    public void RemoveItem(Item item)
    {
        if (item == null) return;
        foreach (InventorySlot slot in Slots)
        {
            if (slot.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
                break;
            }
        }
    }

    /// <summary>
    /// This method removes a 
    /// </summary>
    /// <param name="item"></param>
    public void UseItem(Item item)
    {
        OnItemUsed?.Invoke(item);
        RemoveItem(item);
    }

    /// <summary>
    /// Adds the given ammount of munition to the inventory and fires the
    /// <see cref="OnMunitionUpdate"/> event.
    /// </summary>
    /// <param name="ammount">The ammount of munition the player received0</param>
    public void AddMunition(int ammount)
    {
        currentMunition += ammount;
        OnMunitionUpdate?.Invoke(currentMunition);
    }

    /// <summary>
    /// This method reduces the current ammount of ammunition and fires the
    /// <see cref="OnMunitionUpdate"/> event.
    /// </summary>
    public void UseMunition()
    {
        currentMunition--;
        OnMunitionUpdate?.Invoke(currentMunition);
    }

    /// <summary>
    /// This method finds a slot which contains the type of given item and has still enough space for it.
    /// </summary>
    /// <param name="item">The item which should be added to the inventory.</param>
    /// <returns>The slot for the item.</returns>
    public InventorySlot FindStackableSlot(Item item)
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.IsStackable(item)) return slot;
        }
        return null;
    }

    /// <summary>
    /// This method finds a free slot in the inventory.
    /// </summary>
    /// <returns>The free slot</returns>
    public InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in Slots)
        {
            if (slot.IsEmpty) return slot;
        }
        return null;
    }

    /// <summary>
    /// This method determines whether the inventory contains the given item or not.
    /// </summary>
    /// <param name="item">The item which should be checked.</param>
    /// <returns>True if the inventory contains the given item; otherwise false.</returns>
    public bool HasItem(Item item)
    {
        foreach (InventorySlot slot in Slots)
        {
            if (!slot.IsEmpty && slot.FirstItem.name == item.name) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether all the slots of the inventory are full or not. The
    /// method will return true if this is the case; otherwise false.
    /// </summary>
    public bool IsFull
    {
        get
        {
            foreach (InventorySlot slot in Slots)
            {
                if (slot.IsEmpty || !slot.IsFull) return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Determines whether the inventory contains ammunition or not. The method
    /// will return true if this is the case; otherwise false.
    /// </summary>
    public bool HasMunition
    {
        get { return currentMunition > 0; }
    }
}


/// <summary>
/// Class <c>InventorySlot</c> is used to store a set ammount of items of the same
/// type. Each slot has it unique id which is used in the <see cref="Hotbar"/> class.
/// </summary>
public class InventorySlot
{
    private int id = 0;
    private Stack<Item> stack;

    public InventorySlot(int id)
    {
        this.id = id;
        stack = new Stack<Item>();
    }

    /// <summary>
    /// This method adds the given item to the slot and sets a reference in the item.
    /// </summary>
    /// <param name="item">The item which should be added.</param>
    public void Add(Item item)
    {
        item.slot = this;
        stack.Push(item);
    }

    /// <summary>
    /// This method removes the given item from the slot.
    /// </summary>
    /// <param name="item">The item which should be removed.</param>
    public bool Remove(Item item)
    {
        if (IsEmpty) return false;

        Item first = stack.Peek();
        if (first.name == item.name)
        {
            stack.Pop();
            return true;
        }
        return false;
    }

    /// <summary>
    /// This method determines whether the slot has space for more items or not
    /// based on the set maximum stack size.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>True if the slot has space for more items; otherwise false.</returns>
    public bool IsStackable(Item item)
    {
        if (IsEmpty || !item.isStackable) return false;

        Item first = stack.Peek();
        if (first.name == item.name && stack.Count < item.maxStackSize) return true;

        return false;
    }

    /// <summary>
    /// Returns the first item of the slot.
    /// </summary>
    public Item FirstItem
    {
        get
        {
            if (IsEmpty) return null;
            return stack.Peek();
        }
    }

    /// <summary>
    /// Returns true if the inventory is full; otherwise false.
    /// </summary>
    public bool IsFull
    {
        get { return FirstItem != null && Count == FirstItem.maxStackSize; }
    }

    /// <summary>
    /// Returns true if the inventory is emtpy; otherwise false.
    /// </summary>
    public bool IsEmpty
    {
        get { return Count == 0; }
    }

    /// <summary>
    /// Returns the ammount of items the slot keeps.
    /// </summary>
    public int Count
    {
        get { return stack.Count; }
    }

    /// <summary>
    /// Returns the id of the slot which is used in the <see cref="Hotbar"/> class.
    /// </summary>
    public int Id
    {
        get { return id; }
    }
}
