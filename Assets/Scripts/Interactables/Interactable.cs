using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private bool _isInTrigger;
    public bool _isInteractable = true;
    public UnityEvent OnEnter;
    public UnityEvent OnExit;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && _isInTrigger && _isInteractable)
        {
            Interact();
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Interact");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerManager.Instance.Player)
        {
            _isInTrigger = true;
            OnEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerManager.Instance.Player)
        {
            _isInTrigger = false;
            OnExit?.Invoke();
        }
    }


}
