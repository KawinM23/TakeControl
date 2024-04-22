using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Collider2D _collider;
    [SerializeField] private int damagePerTick;
    [SerializeField] private float _knockbackMultiplier = 1f;
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            var hitDir = Vector2.Dot(-collision.GetContact(0).normal, transform.up) >= 0 ? transform.up : -transform.up;
            health.TakeDamage(damagePerTick, hitDir, _knockbackMultiplier);
        }
    }

}
