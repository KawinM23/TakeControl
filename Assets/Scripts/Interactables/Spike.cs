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
        Debug.Log("Damage Spike");
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(damagePerTick, Vector2.up, _knockbackMultiplier);
        }
    }

}
