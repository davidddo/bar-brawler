﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Category", menuName = "Shop/Category")]
public class Category : ScriptableObject
{
    new public string name;
    public List<ShopItem> items;
}