using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class Currency : MonoBehaviour
    {
        [SerializeField] private int value;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null && collision.CompareTag("Player"))
            {
                ResourceManager.Instance.AddCurrency(value);
                Destroy(gameObject);
            }
        }
    }
}