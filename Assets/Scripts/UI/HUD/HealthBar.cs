﻿using System;
using Utils;
using Items;

/// <summary>
/// Class <c>HealthBar</c> manages the visualisation of the players health bar
/// by subscribing to the <see cref="EntityState.OnDamaged"/> and
/// <see cref="EntityStats.OnHealed"/> events.
/// </summary>
public class HealthBar : ShrinkBar
{
    private EntityStats stats;

    void Start()
    {
        stats = Player.instance.stats;
        if (stats == null) if (stats == null) throw new ArgumentException("Player stats class cannot be null");

        stats.OnDamaged += OnDamaged;
        stats.OnHealed += OnHealed;
    }

    /// <summary>
    /// Gets called if the player got healed and updates the health bar accordingly.
    /// </summary>
    /// <param name="amount">The amount of health points the player received.</param>
    void OnHealed(float amount)
    {
        SetBarFillAmount(stats.HealthNormalized);
        AlignBars();
    }

    /// <summary>
    /// Gets called when the player took damage and updates the health bar accordingly..
    /// </summary>
    /// <param name="amount">The amount of damage the player took.</param>
    void OnDamaged(float damage, Equipment item)
    {
        ResetShrinkTimer();
        SetBarFillAmount(stats.HealthNormalized);
    }
}