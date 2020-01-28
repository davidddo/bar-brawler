﻿using UnityEngine;

namespace Items
{
    public class Revolver : Equippable
    {
        public Bullet bullet;
        public Transform muzzle;
        public MuzzleFlash muzzleFlash;
        public float bulletSpeed;
        public float fireRate = 1f;

        private float cooldown = 0f;

        private EntityAnimator animator;

        void Start()
        {
            animator = Player.instance.animator;
        }

        void Update()
        {
            cooldown -= Time.deltaTime;
        }

        public override void OnPrimary()
        {
            if (Player.instance.inventory.HasMunition && cooldown <= 0f)
            {
                cooldown = 1f / fireRate;

                FindObjectOfType<AudioManager>().Play("Shot");
                muzzleFlash.Play();

                Bullet newBullet = Instantiate(bullet, muzzle.position, muzzle.rotation) as Bullet;
                newBullet.speed = bulletSpeed;
                bullet.OnHit += OnHit;

                if (TargetAcquisition.instance.CurrentEnemy != null)
                {
                    Enemy enemy = TargetAcquisition.instance.CurrentEnemy;
                    Vector3 bulletDirection = enemy.transform.position - muzzle.position;

                    newBullet.transform.rotation = Quaternion.LookRotation(bulletDirection, Vector3.up);
                }

                Player.instance.inventory.UseMunition();
                // animator.OnPrimary();
            }
        }
    }
}
