using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    [RequireComponent(typeof(Controller))]

    public class Gun : MonoBehaviour
    {
        [SerializeField] private float bulletSpeed = 10f; //TODO: confirm design with team
        [SerializeField] private GameObject bulletPrefab;
        private Controller controller;
        private double lastFireTime, lastReloadTime = -1;
        private readonly double shootingDelay = 0.25, reloadTime = 5;
        private uint currentAmmo = 20;
        private readonly uint maxAmmo = 20;

        private void Awake()
        {
            controller = GetComponent<Controller>();
        }

        private void Update()
        {
            controller.input.UpdateInputEventLoop();
            if (lastReloadTime == -1)
            {
                if (controller.input.RetrieveAttackInput())
                {
                    if (Time.fixedTimeAsDouble - lastFireTime >= shootingDelay && currentAmmo > 0)
                    {
                        Shoot();
                        lastFireTime = Time.fixedTimeAsDouble; //TODO: beware when pausing game, should have global time control
                        currentAmmo -= 1;
                    }
                }
                if (controller.input.RetrieveReloadInput())
                {
                    lastReloadTime = Time.fixedTimeAsDouble; //TODO: beware when pausing game, should have global time control
                }
            }
            else if (Time.fixedTimeAsDouble - lastReloadTime >= reloadTime)
            {
                currentAmmo = maxAmmo;
                lastReloadTime = -1;
            }
        }

        public void Shoot()
        {
            Debug.Log("Shoot");

            Vector2 firePoint = transform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 bulletDirection = mousePosition - firePoint;

            GameObject bulletInstance = Instantiate(bulletPrefab, firePoint, Quaternion.identity);
            Bullet bullet = bulletInstance.GetComponent<Bullet>();
            if (bullet)
            {
                if (gameObject)
                {
                    bullet.isEnemy = false;
                }
                bullet.Fire(bulletDirection.normalized * bulletSpeed);
            }
        }
    }
}
