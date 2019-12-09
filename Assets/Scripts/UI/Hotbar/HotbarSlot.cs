﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class HotbarSlot : MonoBehaviour
{
    public Image icon;
    Equippable item;

    public void Add(Equippable item)
    {
        this.item = item;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void Clear()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }
}
