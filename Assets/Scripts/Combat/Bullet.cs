using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Bullet : MonoBehaviour
    {
        public bool IsEnemy;

        [SerializeField] private int _damage = 15;
        [SerializeField] private LayerMask _actionLayer;
        [SerializeField] private LayerMask _destroyLayer;
        private Rigidbody2D _rigidbody;
        private readonly Collider2D _collider;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Fire(Vector2 velocity)
        {
            _rigidbody.velocity = velocity;
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }


        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_destroyLayer == (_destroyLayer | (1 << collider.transform.gameObject.layer)))
            {
                Destroy(gameObject);
            }
            if (_actionLayer == (_actionLayer | (1 << collider.transform.gameObject.layer)))
            {
                bool hitPlayer = collider.gameObject == PlayerManager.Instance.Player;
                // Enemy Bullet
                if (IsEnemy)
                {
                    if (!hitPlayer)
                    {
                        return;
                    }
                    if (collider.gameObject.TryGetComponent(out Health health))
                    {
                        health.TakeDamage(_damage);
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
                        health.TakeDamage(_damage);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}




