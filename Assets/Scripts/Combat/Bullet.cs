using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Bullet : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Collider2D cl;

        public bool isEnemy;
        [SerializeField] private int damage = 15;
        [SerializeField] private LayerMask checkLayer;
        [SerializeField] private LayerMask collideLayer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Fire(Vector2 velocity)
        {
            rb.velocity = velocity;
            StartCoroutine(InitBullet());
        }

        private IEnumerator InitBullet()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collideLayer == (collideLayer | (1 << collider.transform.gameObject.layer)))
            {
                Destroy(gameObject);
            }
            if (checkLayer == (checkLayer | (1 << collider.transform.gameObject.layer)))
            {
                bool hitPlayer = collider.gameObject == PlayerManager.Instance.playerGameObject;
                // Enemy Bullet
                if (isEnemy)
                {
                    if (!hitPlayer)
                    {
                        return;
                    }
                    if (collider.gameObject.TryGetComponent(out Health health))
                    {
                        health.TakeDamage(damage);
                    }
                }
                else// Our Bullet
                {
                    if (hitPlayer)
                    {
                        return;
                    }
                    if (collider.gameObject.TryGetComponent(out Health health))
                    {
                        health.TakeDamage(damage);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}




