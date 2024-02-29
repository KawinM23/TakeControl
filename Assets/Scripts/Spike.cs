using Assets.Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Collider2D collider;
    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("Damage Spkie");
        collision.gameObject.TryGetComponent(out Health health);
        if (health)
        {
            health.TakeDamage(34);
        }
    }

}
