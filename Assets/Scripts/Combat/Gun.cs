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

        private void Awake()
        {
            controller = GetComponent<Controller>();
        }

        private void Update()
        {
            if (controller.input.RetrieveAttackInput())
            {
                Shoot();
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
                bullet.Fire(bulletDirection.normalized * bulletSpeed);
            }
        }
    }
}
