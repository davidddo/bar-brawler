﻿using UnityEngine;
using UnityEngine.UI;
using Items;

public class HotbarSlot : MonoBehaviour
{
    [Header("Sprites")]
    public Image background;
    public Sprite selectedSprite;
    public Sprite defaultSprite;

    [Header("Item")]
    public Item item;
    public Image icon;
    public Sprite iconPlaceholder;
    public Text count;

    public bool IsDragAndDropEnabled { get; set; } = true;

    public void Add(Item item)
    {
        this.item = item;

        icon.sprite = item.icon;
        icon.color = Color.white;
        icon.enabled = true;

        if (item.isStackable)
        {
            count.gameObject.SetActive(true);
            count.text = item.slot.Count.ToString();
        }
    }

    public void Clear()
    {
        item = null;

        icon.color = Color.clear;
        icon.sprite = iconPlaceholder;
        
        count.gameObject.SetActive(true);
        count.text = "";
    }

    public void UpdateCount(int currenCount)
    {
        count.text = currenCount.ToString();
    }

    public void SetSelected(bool selected)
    {
        background.sprite = selected ? selectedSprite : defaultSprite;
    }
}
