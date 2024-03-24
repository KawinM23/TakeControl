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
        [SerializeField] private float _knockbackMultiplier = 1f;
        [SerializeField] private Collider2D _swordCollider;
        [SerializeField] private LayerMask _attackableLayer;

        [Header("Animation")]
        [Tooltip("Radial of slash animation over time")]
        [SerializeField] private AnimationCurve _radialCurve;
        [Tooltip("Duration of the slash animation")]
        [SerializeField] private float _animationDuration = 0.2f;

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
            SoundManager.Instance.PlaySlash();
            Vector2 direction = (mousePosition - (Vector2)_parentTransform.position).normalized;

            _swordCollider.transform.localPosition = direction * 1.2f;
            _swordCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
        private IEnumerator SwordAnimation()
        {
            var mask = GetComponentInChildren<SpriteMask>();

            _swordCollider.enabled = true;
            _sprite.enabled = true;

            for (float t = 0; t <= _animationDuration; t += Time.fixedDeltaTime)
            {
                mask.alphaCutoff = _radialCurve.Evaluate(t / _animationDuration);
                yield return new WaitForFixedUpdate();
            }

            _swordCollider.enabled = false;
            _sprite.enabled = false;
            mask.alphaCutoff = 0f;


            // _swordCollider.enabled = true;
            // _sprite.enabled = true;
            // yield return new WaitForSeconds(0.2f);
            // _swordCollider.enabled = false;
            // _sprite.enabled = false;
            // yield return null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            if (collision.gameObject != gameObject && collision.TryGetComponent(out Health health))
            {
                Debug.Log(collision);

                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                health.TakeDamage(_swordDamage, hitDirection, _knockbackMultiplier);
                SoundManager.Instance.PlaySwordImpact();

            }

        }
    }
}
