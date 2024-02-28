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
        [SerializeField] private int damage = 15;
        [SerializeField] private LayerMask collideLayer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void Fire(Vector2 velocity)
        {
            rb.velocity = velocity;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            Controller controller = collider.gameObject.GetComponent<Controller>();
            // If it hits enemy
            if (controller && controller.input is AIController)
            {
                collider.gameObject.GetComponent<Health>().TakeDamage(damage);
                Destroy(gameObject);
            }
            else if ((collideLayer.value & (1 << collider.transform.gameObject.layer)) > 0)
            {
                Destroy(gameObject);
            }
        }

    }



}