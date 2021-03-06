﻿using UnityEngine;

namespace Items
{
    public class Bullet : MonoBehaviour
    {
        public delegate void Hit(Enemy enemy);
        public event Hit OnHit;

        public float speed;

        void Start()
        {
            Destroy(gameObject, 10f);
        }

        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.tag);

            if (other.gameObject.tag == "Enemy")
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                if (enemy != null)  OnHit?.Invoke(enemy);
            }
        }
    }
}
