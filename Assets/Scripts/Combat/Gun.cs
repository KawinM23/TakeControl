using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Gun : BaseWeapon
    {
        [SerializeField] private float _bulletSpeed = 40f; // TODO: confirm design with team
        [SerializeField] private float _knockbackMultiplier = 0.7f;
        [SerializeField] private GameObject _bulletPrefab;
        private Controller _controller;
        private double _lastFireTime, _lastReloadTime = -1;
        [SerializeField] private double _shootingDelay = 0.25, _reloadTime = 5;
        private uint _currentAmmo = 20;
        public uint CurrentAmmo => _currentAmmo;
        private readonly uint _maxAmmo = 20;
        public uint MaxAmmo
        {
            get { return _maxAmmo; }
        }
        public bool Reloading => _lastReloadTime != -1;

        protected override void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        protected override void Update()
        {
            if (_lastReloadTime == -1)
            {
                Vector2? pos = null;
                if (_controller.Input.GetAttackDirection().HasValue)
                {
                    pos = _controller.Input.GetAttackDirection().Value;
                }
                else if (_controller.Input.GetContinuedAttackDirection().HasValue)
                {
                    pos = _controller.Input.GetContinuedAttackDirection().Value;
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
                if (_controller.Input.IsReloadPressed())
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
                bullet.Fire(bulletDirection.normalized * _bulletSpeed, _knockbackMultiplier);
                bullet.IsEnemy = gameObject.tag != "Player";
            }
        }

        public bool IsReloading()
        {
            return _lastReloadTime != -1;
        }

        public double GetCurrentReloadPercent()
        {
            if (!IsReloading())
            {
                return 100;
            }
            else
            {
                double reloadPercent = ((Time.fixedTimeAsDouble - _lastReloadTime) / _reloadTime) * 100;
                reloadPercent = System.Math.Min(reloadPercent, 100);
                reloadPercent = System.Math.Max(reloadPercent, 0);
                return reloadPercent;
            }
        }
    }
}
