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
        private double lastFireTime;
        private readonly double shootingDelay = 0.25;

        private void Awake()
        {
            controller = GetComponent<Controller>();
        }

        private void Update()
        {
            controller.input.UpdateInputEventLoop();
            if (controller.input.RetrieveAttackInput())
            {
                if (Time.fixedTimeAsDouble - lastFireTime >= shootingDelay)
                {
                    Shoot();
                    lastFireTime = Time.fixedTimeAsDouble; //TODO: beware when pausing game, should have global time control
                }
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
