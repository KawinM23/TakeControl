using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Combat
{
    public class Health : MonoBehaviour
    {
        public UnityEvent OnHackable;

        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private int hackableHealth = 20;

        [Header("iFrame")]
        [SerializeField] private bool iFrame = false;
        [SerializeField] private float iFrameDuration;
        private float iFrameCounter;
        [SerializeField] private LayerMask iFramableLayer;

        private SpriteRenderer spriteRenderer;
        private Collider2D collider;

        private Coroutine flashCoroutine;
        IEnumerator Flash(Color targetColor)
        {
            var originalColor = spriteRenderer.color;
            spriteRenderer.color = targetColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            flashCoroutine = null;
        }

        private void Awake()
        {

        }

        // Use this for initialization
        private void Start()
        {
            currentHealth = maxHealth;
            iFrame = false;
            iFrameCounter = 0f;
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (currentHealth <= 0)
            {
                Die();
            }
            if (Hackable())
            {
                OnHackable?.Invoke();
            }
        }
        private void FixedUpdate()
        {
            if (iFrame)
            {
                iFrameCounter -= Time.fixedDeltaTime;
            }
            if (iFrameCounter <= 0f)
            {
                iFrame = false;
            }

        }
        public void TakeDamage(int damage)
        {
            if (iFrame)
            {
                return;
            }
            // TODO: for debug, remove of refactor
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(Flash(Color.red));

            currentHealth -= damage;
            iFrame = true;
            iFrameCounter = iFrameDuration;
        }

        public void Heal(int healAmount)
        {
            currentHealth += healAmount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Does the health meet the requirements to be hacked
        /// </summary>
        public bool Hackable() => currentHealth <= hackableHealth;
    }
}