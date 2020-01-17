﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : Equippable
{
    public Bullet bullet;
    public Transform muzzle;
    public ParticleSystem muzzleFlash;
    public float bulletSpeed;
    public float fireRate = 1f;

    private float cooldown = 0f;

    private Inventory inventory;
    private PlayerAnimator animator;

    void Start()
    {
        inventory = Player.instance.inventory;
        animator = Player.instance.animator;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
    }

    public override void OnInteractPrimary()
    {
        if (cooldown <= 0f)
        {
            cooldown = 1f / fireRate;

            muzzleFlash.Play();

            Bullet newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as Bullet;
            newBullet.speed = bulletSpeed;

            inventory.UseAmmunition();
            animator.OnPrimary();
        }
    }
}
