using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Collider2D _collider;
    [SerializeField] private int damagePerTick;
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Damage Spike");
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(damagePerTick);

            // TODO: temp knockback, remove later
            health.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        }
    }

}
