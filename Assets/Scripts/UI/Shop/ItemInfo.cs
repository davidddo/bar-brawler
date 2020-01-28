﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Shop;
using System.Collections;

public class ItemInfo : FadeGraphic
{
    public Text title;
    public Image image;
    public TextMeshProUGUI info;
    public Button buyButton;

    public Text eventText;

    private ShopItem shopItem;

    public void SetItem(ShopItem shopItem)
    {
        this.shopItem = shopItem;

        gameObject.SetActive(true);

        title.text = shopItem.item.name.ToUpper();
        image.sprite = shopItem.item.icon;
        info.text = shopItem.infoText.ToUpper();
        buyButton.GetComponent<Text>().text = "BUY FOR $" + shopItem.price.ToString();
    }

    public void OnItemBought()
    {
        StopAllCoroutines();
        eventText.text = "";
        eventText.color = Color.white;

        FadeIn(eventText, .5f);

        if (Player.instance.currentBalance < shopItem.price)
        {
            eventText.text = "Du hast nicht genug Geld!".ToUpper();
            StartCoroutine(HideEventText());
            return;
        }

        if (Player.instance.inventory.HasItem(shopItem.item) && Player.instance.inventory.FindStackableSlot(shopItem.item) == null) 
        {
            eventText.text = "Du hast schon zu viele Items dieser Art".ToUpper();
            StartCoroutine(HideEventText());
            return;
        }


        shopItem.OnItemBought();
    }

    private IEnumerator HideEventText()
    {
        yield return new WaitForSeconds(2f);
        FadeOut(eventText, .5f);
        yield return new WaitForSeconds(.5f);

        eventText.text = "";
    }
}
