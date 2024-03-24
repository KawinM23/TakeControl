using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    private Collider2D _collider;

    private void Awake()
    {
        _collider = transform.parent.GetComponent<Collider2D>();
    }

    public void DropPlayer()
    {
        Physics2D.IgnoreCollision(PlayerManager.Instance.Player.GetComponent<Collider2D>(), _collider, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(PlayerManager.Instance.Player.GetComponent<Collider2D>(), _collider, false);
        }
    }

}
