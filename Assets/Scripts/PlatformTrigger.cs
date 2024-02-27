using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    private Collider2D platformCollider;

    private GameObject player;

    private void Awake()
    {
        platformCollider = transform.parent.GetComponent<Collider2D>();
    }

    private void Start()
    {
        player = Player.FindActivePlayer();
    }

    public void DropPlayer()
    {
        player = Player.FindActivePlayer();
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), platformCollider, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), platformCollider, false);
        }
    }

}
