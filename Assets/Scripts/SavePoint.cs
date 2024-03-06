using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    private bool inTrigger;
    private bool interactable = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && inTrigger && interactable)
        {
            Debug.Log("Save");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerManager.Instance.playerGameObject)
        {
            inTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerManager.Instance.playerGameObject)
        {
            inTrigger = false;
        }
    }
}
