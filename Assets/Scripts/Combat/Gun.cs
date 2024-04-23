using Assets.Scripts.Effect;
using Assets.Scripts.SaveLoad;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Gun : BaseWeapon, ISavePersist
    {
        private Controller _controller;

        /// <summary>
        /// bullet speed in units per second
        /// 10: normal we use in everything
        /// 20: fast
        /// 40: brrr
        /// </summary>
        [SerializeField] protected int _bulletDamage = 15; // TODO: confirm design with team
        [SerializeField] protected float _bulletSpeed = 40f; // TODO: confirm design with team
        [SerializeField] protected float _knockbackMultiplier = 0.7f;
        [SerializeField] protected GameObject _bulletPrefab;

        /// <summary>
        /// bullet spread in degrees
        /// 1: you drunk fuck
        /// 0.5: skill issue
        /// 0.2: balanced
        /// 0.1: accurate
        /// 0.05 - 0: sniper
        /// </summary>
        [SerializeField] protected float _bulletSpread;

        protected double _shootTimer, _reloadTimer = 0;
        [SerializeField] protected double _shootingDelay = 0.25, _reloadTime = 2;
        public uint MaxAmmo = 20;
        public uint CurrentAmmo = 20;

        public bool Reloading;
        protected bool _unlimitedAmmo = false;

        [SerializeField] protected bool _forceBulletDamage = false;

        protected override void Awake()
        {
            _controller = GetComponent<Controller>();
        }

        protected override void Update()
        {

            if (_shootTimer > 0)
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
                if (_shootTimer <= 0 && (_unlimitedAmmo || CurrentAmmo > 0) && Time.timeScale != 0f)
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
                    CurrentAmmo = MaxAmmo;
                    _shootTimer = 0;
                }
            }
        }

        public virtual void Shoot(Vector2 target)
        {
            if (HackEventManager.Instance != null && HackEventManager.Instance.IsHacking)
            {
                return;
            }

            // Screen Shake if bullet is big
            if (_bulletPrefab.transform.localScale.x >= 0.3)
            {
                ScreenShake.Shake(ScreenShake.ShakeType.ShootBigBullet);
            }

            _shootTimer = _shootingDelay;
            CurrentAmmo -= _unlimitedAmmo ? 0 : (uint)1;
            Reloading = false;
            if (SoundManager.Instance != null) SoundManager.Instance.PlayShoot();
            Vector2 firePoint = transform.position;
            Vector2 bulletDirection = target - firePoint;

            // add spread to bullet direction
            bulletDirection = new Vector2(bulletDirection.x + Random.Range(-_bulletSpread, _bulletSpread), bulletDirection.y + Random.Range(-_bulletSpread, _bulletSpread));

            GameObject bulletInstance = Instantiate(_bulletPrefab, firePoint, Quaternion.identity);
            if (bulletInstance.TryGetComponent(out Bullet bullet))
            {
                bullet.Fire(bulletDirection.normalized * _bulletSpeed, _knockbackMultiplier, _bulletDamage);
                if (PlayerManager.Instance) bullet.IsEnemy = PlayerManager.Instance.Player != gameObject;
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

        public bool GetUnlimitedAmmo()
        {
            return _unlimitedAmmo;
        }

        public void SetUnlimitedAmmo(bool unlimitedAmmo)
        {
            _unlimitedAmmo = unlimitedAmmo;
        }

        public float GetBulletSpeed()
        {
            return _bulletSpeed;
        }

        public void SetBulletSpeed(float bulletSpeed)
        {
            _bulletSpeed = bulletSpeed;
        }

        public double GetShootingDelay()
        {
            return _shootingDelay;
        }

        public void SetShootingDelay(double shootingDelay)
        {
            _shootingDelay = shootingDelay;
        }

        public void SetKnockbackMultiplier(float knockbackMultiplier)
        {
            _knockbackMultiplier = knockbackMultiplier;
        }

        public void SetShootingDelay(float shootingDelay)
        {
            _shootingDelay = shootingDelay;
        }

        public void SetReloadTime(float reloadTime)
        {
            _reloadTime = reloadTime;
        }

        public void SaveData(ref GameData data)
        {
            // Only deal with the player
            if (gameObject != PlayerManager.Instance.Player)
            {
                return;
            }

            // Store data
            data.modules.gun = new ModulesData.Gun
            {
                bulletSpeed = _bulletSpeed,
                shootingDelay = _shootingDelay,
                knockbackMultiplier = _knockbackMultiplier,
                reloadTime = _reloadTime,
                unlimitedAmmo = _unlimitedAmmo
            };
        }

        public void LoadData(in GameData data)
        {
            // Only deal with the player
            if (gameObject != PlayerManager.Instance.Player)
            {
                return;
            }

            var m = data.modules.gun;
            if (m == null)
            {
                // No saved data, remove this component
                DestroyImmediate(this);
                return;
            }

            // Apply saved data
            _bulletSpeed = m.bulletSpeed;
            _shootingDelay = m.shootingDelay;
            _knockbackMultiplier = m.knockbackMultiplier;
            _reloadTime = m.reloadTime;
            _unlimitedAmmo = m.unlimitedAmmo;

        }
    }
}
