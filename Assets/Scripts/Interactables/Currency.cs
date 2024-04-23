using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Interactables
{
    public class Currency : MonoBehaviour
    {
        [SerializeField] private int _value;

        public void SetValue(int value)
        {
            this._value = value;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision != null && collision.gameObject.CompareTag("Player"))
            {
                ResourceManager.Instance.AddCurrency(_value);
                SoundManager.Instance.PlayDing(Random.Range(1,4),Random.Range(5,6));
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null && collision.CompareTag("Player"))
            {
                ResourceManager.Instance.AddCurrency(_value);
                SoundManager.Instance.PlayDing(Random.Range(1,4),Random.Range(5,6));
                Destroy(gameObject);
            }
        }
    }
}