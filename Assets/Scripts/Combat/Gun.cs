using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Gun : MonoBehaviour
    {
        [SerializeField] private float _bulletSpeed = 10f; //TODO: confirm design with team
        [SerializeField] private GameObject _bulletPrefab;
        private Controller _controller;
        private double _lastFireTime, _lastReloadTime = -1;
        private readonly double _shootingDelay = 0.25, _reloadTime = 5;
        private uint _currentAmmo = 20;
        private readonly uint _maxAmmo = 20;

        private void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        private void Update()
        {
            if (_lastReloadTime == -1)
            {
                Vector2? pos = null;
                if (_controller.input.GetAttackDirection().HasValue)
                {
                    pos = _controller.input.GetAttackDirection().Value;
                }
                else if (_controller.input.GetContinuedAttackDirection().HasValue)
                {
                    pos = _controller.input.GetContinuedAttackDirection().Value;
                }
                if (pos != null)
                {
                    if (Time.fixedTimeAsDouble - _lastFireTime >= _shootingDelay && _currentAmmo > 0)
                    {
                        Shoot(pos.Value);
                        _lastFireTime = Time.fixedTimeAsDouble; //TODO: beware when pausing game, should have global time control
                        _currentAmmo -= 1;
                    }
                }
                if (_controller.input.IsReloadPressed())
                {
                    _lastReloadTime = Time.fixedTimeAsDouble; //TODO: beware when pausing game, should have global time control
                }
            }
            else if (Time.fixedTimeAsDouble - _lastReloadTime >= _reloadTime)
            {
                _currentAmmo = _maxAmmo;
                _lastReloadTime = -1;
            }
        }

        public void Shoot(Vector2 target)
        {
            Debug.Log("Shoot");
            SoundManager.Instance.PlayShoot();
            Vector2 firePoint = transform.position;
            Vector2 bulletDirection = target - firePoint;

            GameObject bulletInstance = Instantiate(_bulletPrefab, firePoint, Quaternion.identity);
            Bullet bullet = bulletInstance.GetComponent<Bullet>();
            if (bullet)
            {
                if (gameObject)
                {
                    bullet.IsEnemy = false;
                }
                bullet.Fire(bulletDirection.normalized * _bulletSpeed);
            }
        }
    }
}
