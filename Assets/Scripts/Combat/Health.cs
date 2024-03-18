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

        private SpriteRenderer _spriteRenderer;
        private readonly Collider2D _collider;

        private Coroutine flashCoroutine;
        IEnumerator Flash(Color targetColor)
        {
            var originalColor = _spriteRenderer.color;
            _spriteRenderer.color = targetColor;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            flashCoroutine = null;
        }

        // Use this for initialization
        private void Start()
        {
            _currentHealth = _maxHealth;
            _iFrame = false;
            _iFrameCounter = 0f;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_currentHealth <= 0)
            {
                Die();
            }
            if (IsHackable())
            {
                OnHackable?.Invoke();
            }
        }
        private void FixedUpdate()
        {
            if (_iFrame)
            {
                _iFrameCounter -= Time.fixedDeltaTime;
            }
            if (_iFrameCounter <= 0f)
            {
                _iFrame = false;
            }

        }
        public void TakeDamage(int damage)
        {
            if (_iFrame)
            {
                return;
            }
            // TODO: for debug, remove of refactor
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(Flash(Color.red));

            _currentHealth -= damage;
            _iFrame = true;
            _iFrameCounter = _iFrameDuration;
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        public int GetMaxHealth()
        {
            return this.maxHealth;
        }
        public int GetCurrentHealth()
        {
            return this.currentHealth;
        }
        /// <summary>
        /// Does the health meet the requirements to be hacked
        /// </summary>
        public bool IsHackable() => _currentHealth <= _hackableHealth;
    }
}