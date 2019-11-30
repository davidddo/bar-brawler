﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Item
{
    public int amount;

    public override void OnCollection()
    {
        base.OnCollection();
        Player.instance.AddMoney(amount);
    }
}
