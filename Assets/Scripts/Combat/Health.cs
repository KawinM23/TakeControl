using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Combat
{
    public class Health : MonoBehaviour
    {
        public UnityEvent OnHackable;

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;
        [SerializeField] private int _hackableHealth = 20;

        [Header("iFrame")]
        [SerializeField] private bool _iFrame = false;
        [SerializeField] private float _iFrameDuration;
        private float _iFrameCounter;
        [SerializeField] private LayerMask _iFramableLayer;
        [SerializeField] private PlayerManager _playerManager;


        private SpriteRenderer _spriteRenderer;
        private readonly Collider2D _collider;
        private Rigidbody2D _rigidbody;
        private Coroutine _flashCoroutine;
        private Color _originalColor;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        // Use this for initialization
        private void Start()
        {
            _currentHealth = _maxHealth;
            _iFrame = false;
            _iFrameCounter = 0f;
            _spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
            _playerManager = PlayerManager.Instance;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_iFrame)
            {
                _iFrameCounter -= Time.deltaTime;
                if (_iFrameCounter <= 0f)
                {
                    _iFrame = false;
                }
            }
            if (_currentHealth <= 0)
            {
                Die();
            }
            if (IsHackable() && gameObject != PlayerManager.Instance.Player)
            {
                OnHackable?.Invoke();
            }
        }

        public void ResetHealth()
        {
            _currentHealth = _maxHealth;
        }


        public void TakeDamage(int damage, Vector2 hitDirection)
        {
            if (_iFrame)
            {
                return;
            }
            // TODO: for debug, remove of refactor
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                AfterFlash();
            }
            _flashCoroutine = StartCoroutine(Flash(Color.red));


            ApplyKnockback(hitDirection, damage);

            _currentHealth -= damage;
            _iFrame = true;
            _iFrameCounter = _iFrameDuration;
        }

        IEnumerator Flash(Color targetColor)
        {
            _originalColor = _spriteRenderer.color;
            _spriteRenderer.color = targetColor;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = _originalColor;
            yield return new WaitForSeconds(0.1f);
            AfterFlash();
        }

        void AfterFlash()
        {
            _spriteRenderer.color = _originalColor;
            _flashCoroutine = null;
        }

        private void Die()
        {
            if (gameObject == PlayerManager.Instance.Player)
            {
                _playerManager.Die();
            }
            Destroy(gameObject);
        }

        public int GetMaxHealth()
        {
            return this._maxHealth;
        }
        public int GetCurrentHealth()
        {
            return this._currentHealth;
        }
        /// <summary>
        /// Does the health meet the requirements to be hacked
        /// </summary>
        public bool IsHackable() => _currentHealth <= _hackableHealth;

        public void ApplyKnockback(Vector2 hitDirection, float damage)
        {
            // The more damage, the more knockback force is applied
            float knockbackForce = 500f * damage / _maxHealth;

            _rigidbody.AddForce(hitDirection * knockbackForce, ForceMode2D.Force);
            _rigidbody.AddForce(Vector2.up * knockbackForce / 2, ForceMode2D.Force);
        }
    }


}