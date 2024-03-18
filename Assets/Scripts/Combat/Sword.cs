using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]
    public class Sword : MonoBehaviour
    {
        public UnityEvent OnAttack;

        [SerializeField] private int _swordDamage = 25;
        [SerializeField] private Collider2D _swordCollider;
        [SerializeField] private LayerMask _attackableLayer;
        private SpriteRenderer _sprite;

        private Controller _controller;
        private Transform _parentTransform;

        private readonly Collider2D[] _hitEnemies;

        private void Awake()
        {
            _controller = GetComponent<Controller>();
            _parentTransform = _swordCollider.transform.parent;
            _sprite = _swordCollider.GetComponent<SpriteRenderer>();
            _sprite.enabled = false;
            _swordCollider.enabled = false;
        }

        private void Update()
        {
            if (_controller.input.GetAttackDirection().HasValue)
            {
                AttackAction(_controller.input.GetAttackDirection().Value);
                OnAttack?.Invoke();
            }
        }

        private void AttackAction(Vector2 mousePosition)
        {
            StartCoroutine(SwordAnimation());
            Debug.Log("Sword Attack");
            Vector2 direction = (mousePosition - (Vector2)_parentTransform.position).normalized;

            _swordCollider.transform.localPosition = direction * 1.2f;
            _swordCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
        private IEnumerator SwordAnimation()
        {
            _swordCollider.enabled = true;
            _sprite.enabled = true;
            yield return new WaitForSeconds(0.2f);
            _swordCollider.enabled = false;
            _sprite.enabled = false;
            yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject != gameObject && collision.TryGetComponent(out Health health))
            {
                Debug.Log(collision);
                health.TakeDamage(_swordDamage);
            }

        }
    }
}
