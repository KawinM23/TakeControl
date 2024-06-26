using Assets.Scripts.Effect;
using Assets.Scripts.SaveLoad;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]
    public class Sword : BaseWeapon, ISavePersist
    {
        public UnityEvent OnAttack;

        [SerializeField] private int _swordDamage = 25;
        [SerializeField] private float _attackCooldown = 0.5f;

        [SerializeField] private float _knockbackMultiplier = 1f;
        [SerializeField] private Collider2D _swordCollider;
        [SerializeField] private LayerMask _attackableLayer;

        [Header("Animation")]
        [Tooltip("Radial of slash animation over time")]
        [SerializeField] public AnimationCurve _radialCurve;
        [Tooltip("Duration of the slash animation")]
        [SerializeField] private float _animationDuration = 0.2f;

        private double _attackTimer = -1;

        private SpriteRenderer _sprite;
        private SpriteMask _mask; // for radial slash animation

        private Controller _controller;
        private Transform _parentTransform;

        private readonly Collider2D[] _hitEnemies;

        protected override void Awake()
        {
            _controller = GetComponent<Controller>();
            _parentTransform = _swordCollider.transform.parent;
            _sprite = _swordCollider.GetComponent<SpriteRenderer>();
            _mask = _swordCollider.GetComponentInChildren<SpriteMask>();
        }

        private void Start()
        {
            AfterSwordAnimation();
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode loadSceneMode) => { if (_swordCollider) _swordCollider.gameObject.SetActive(false); };
        }

        protected override void Update()
        {
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }
            if (_controller.Input.GetAttackDirection().HasValue && _attackTimer <= 0 && Time.timeScale != 0f)
            {
                AttackAction(_controller.Input.GetAttackDirection().Value);
                OnAttack?.Invoke();
            }
        }

        private void AttackAction(Vector2 mousePosition)
        {
            if (HackEventManager.Instance.IsHacking)
            {
                return;
            }

            ScreenShake.Shake(ScreenShake.ShakeType.Attack);
            StartCoroutine(SwordAnimation());

            Debug.Log("Sword Attack");
            _attackTimer = _attackCooldown;
            SoundManager.Instance.PlaySlash();
            Vector2 direction = (mousePosition - (Vector2)_parentTransform.position).normalized;

            _swordCollider.transform.localPosition = direction * 1.2f;
            _swordCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
        private IEnumerator SwordAnimation()
        {
            if (TryGetComponent(out Health health)) _sprite.color = health.OriginalColor;
            _sprite.gameObject.SetActive(true);
            _mask.enabled = true;
            _swordCollider.enabled = true;
            _sprite.enabled = true;

            for (float t = 0; t <= _animationDuration; t += Time.fixedDeltaTime)
            {
                _mask.alphaCutoff = _radialCurve.Evaluate(t / _animationDuration);
                yield return new WaitForFixedUpdate();
            }

            AfterSwordAnimation();

            // _swordCollider.enabled = true;
            // _sprite.enabled = true;
            // yield return new WaitForSeconds(0.2f);
            // _swordCollider.enabled = false;
            // _sprite.enabled = false;
            // yield return null;
        }

        private void AfterSwordAnimation()
        {
            _mask.alphaCutoff = 0f;
            _mask.enabled = false;
            _sprite.enabled = false;
            _swordCollider.enabled = false;
            _sprite.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != gameObject && collision.TryGetComponent(out Health health))
            {
                Debug.Log("Sword hit " + collision);

                Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                health.TakeDamage(_swordDamage, hitDirection, _knockbackMultiplier);
                SoundManager.Instance.PlaySwordImpact();
            }

        }

        public void SaveData(ref GameData data)
        {
            // Only deal with the player
            if (gameObject != PlayerManager.Instance.Player)
            {
                return;
            }

            // Store data
            data.modules.sword = new ModulesData.Sword
            {
                swordDamage = _swordDamage,
                attackCooldown = _attackCooldown,
                knockbackMultiplier = _knockbackMultiplier
            };
        }

        public void LoadData(in GameData data)
        {
            // Only deal with the player
            if (gameObject != PlayerManager.Instance.Player)
            {
                return;
            }

            var m = data.modules.sword;
            if (m == null)
            {
                // No saved data, remove this component
                DestroyImmediate(this);
                return;
            }

            // Apply saved data
            _swordDamage = m.swordDamage;
            _attackCooldown = m.attackCooldown;
            _knockbackMultiplier = m.knockbackMultiplier;
        }
    }
}
