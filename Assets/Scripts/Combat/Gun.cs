using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Gun : BaseWeapon
    {
        private Controller _controller;

        [SerializeField] private float _bulletSpeed = 40f; // TODO: confirm design with team
        [SerializeField] private float _knockbackMultiplier = 0.7f;
        [SerializeField] private GameObject _bulletPrefab;

        private double _shootTimer, _reloadTimer = 0;
        [SerializeField] private double _shootingDelay = 0.25, _reloadTime = 5;
        private uint _currentAmmo = 20;
        public uint CurrentAmmo => _currentAmmo;
        private readonly uint _maxAmmo = 20;
        public uint MaxAmmo
        {
            get { return _maxAmmo; }
        }
        public bool Reloading;

        protected override void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        protected override void Update()
        {

            if (_shootTimer >= 0)
            {
                _shootTimer -= Time.deltaTime;
            }
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
                if (_shootTimer <= 0 && _currentAmmo > 0)
                {
                    Shoot(pos.Value);
                }
            }
            if (!Reloading && (_controller.Input.IsReloadPressed() || CurrentAmmo == 0))
            {
                Reloading = true; 
                _reloadTimer = _reloadTime;
            }


            if (Reloading)
            {
                _reloadTimer -= Time.deltaTime;
                if (_reloadTimer <= 0)
                {
                    Reloading = false;
                    _currentAmmo = _maxAmmo;
                    _shootTimer = 0;
                }
            }
        }

        public void Shoot(Vector2 target)
        {
            if (HackEventManager.Instance.IsHacking)
            {
                return;
            }
            Debug.Log("Shoot");
            _shootTimer = _shootingDelay;
            _currentAmmo -= 1;
            Reloading = false;
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
            return Reloading;
        }

        public double GetCurrentReloadPercent()
        {
            if (!IsReloading())
            {
                return 100;
            }
            else
            {
                double reloadPercent = ((_reloadTime - _reloadTimer) / _reloadTime) * 100;
                reloadPercent = System.Math.Min(reloadPercent, 100);
                reloadPercent = System.Math.Max(reloadPercent, 0);
                return reloadPercent;
            }
        }
    }
}
