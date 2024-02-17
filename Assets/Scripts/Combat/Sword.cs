using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]
    public class Sword : MonoBehaviour
    {
        [SerializeField] private int _swordDamage = 25;
        [SerializeField] private Collider2D _swordCollider;
        [SerializeField] private LayerMask _attackableLayer;

        private Controller _controller;

        private Collider2D[] _hitEnemies;

        private void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        private void Update()
        {
            if (_controller.input.RetrieveAttackInput())
            {
                AttackAction();
            }
        }

        private void AttackAction()
        {
            Debug.Log("Attack");
            _hitEnemies = Physics2D.OverlapBoxAll(_swordCollider.transform.position, _swordCollider.transform.lossyScale, 0, _attackableLayer);

            foreach (Collider2D collider in _hitEnemies) {
                Debug.Log(collider);
                collider.GetComponent<Health>().TakeDamage(_swordDamage);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_swordCollider.transform.position, _swordCollider.transform.lossyScale);
        }
    }
}
