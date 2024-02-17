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

        private void Awake()
        {
                      
        }

        // Use this for initialization
        private void Start()
        {
            _currentHealth = _maxHealth;  
        }

        // Update is called once per frame
        private void Update()
        {
            if (_currentHealth<=0)
            {
                Die();
            }
        }
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}