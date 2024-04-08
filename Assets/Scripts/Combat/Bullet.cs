using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Bullet : BaseProjectile
    {
        [SerializeField] private int _damage = 15;
        [SerializeField] private LayerMask _actionLayer;
        [SerializeField] private LayerMask _destroyLayer;
        private Rigidbody2D _rigidbody;
        private float _knockbackMultiplier; // set from gun

        private void Start()
        {
            Damage = _damage;
            ActionLayer = _actionLayer;
            DestroyLayer = _destroyLayer;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public void Fire(Vector2 velocity, float knockbackMultiplier)
        {
            _rigidbody.velocity = velocity;
            _knockbackMultiplier = knockbackMultiplier;
            StartCoroutine(Init());
        }

        private IEnumerator Init()
        {
            yield return new WaitForSeconds(5);
            Destroy(gameObject);
        }

        protected override void OnEnemyHitPlayerAction(GameObject player, Vector3 hitPosition)
        {
            if (player.TryGetComponent(out Health health))
            {
                Vector2 hitDirection = (hitPosition - transform.position).normalized;
                health.TakeDamage(_damage, hitDirection, _knockbackMultiplier);
            }
        }

        protected override void OnPlayerHitEnemyAction(GameObject enemy, Vector3 hitPosition)
        {
            if (enemy.TryGetComponent(out Health health))
            {

                Vector2 hitDirection = _rigidbody.velocity.normalized;
                health.TakeDamage(_damage, hitDirection, _knockbackMultiplier);
                SoundManager.Instance.PlayBulletImpact();
            }
        }
    }
}




