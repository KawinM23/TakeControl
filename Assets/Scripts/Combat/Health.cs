using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public class Health : MonoBehaviour
    {

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentHealth;
        [SerializeField] private int _HackableHealth = 20;

        private SpriteRenderer _spriteRenderer;

        private Coroutine flashCoroutine;
        IEnumerator Flash(Color targetColor)
        {
            var originalColor = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().color = targetColor;
            yield return new WaitForSeconds(0.1f);
            GetComponent<SpriteRenderer>().color = originalColor;
            flashCoroutine = null;
        }

        private void Awake()
        {

        }

        // Use this for initialization
        private void Start()
        {
            _currentHealth = _maxHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Hackable() && _spriteRenderer != null && flashCoroutine == null)
            {
                _spriteRenderer.color = Color.green;
            }

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        public void TakeDamage(int damage)
        {
            // TODO: for debug, remove of refactor
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(Flash(Color.yellow));

            _currentHealth -= damage;
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Does the health meet the requirements to be hacked
        /// </summary>
        public bool Hackable() => _currentHealth <= _HackableHealth;
    }
}