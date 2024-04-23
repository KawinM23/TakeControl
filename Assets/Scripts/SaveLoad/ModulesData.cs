using UnityEngine;

#nullable enable annotations

namespace Assets.Scripts.SaveLoad
{

    [System.Serializable]
    public class ModulesData
    {
        public Health? health;
        public Move? move;
        public Jump? jump;
        public Sword? sword;
        public Gun? gun;

        [System.Serializable]
        public class Health
        {
            public int maxHealth;
            public float defaultKnockbackForce;
            public float iFrameDuration;
        }

        [System.Serializable]
        public class Move
        {
            public bool hasDash;
        }

        [System.Serializable]
        public class Jump
        {
            public float jumpHeight;
            public int maxAirJumps;
            public float downwardMovementMultiplier;
            public float upwardMovementMultiplier;
            public float coyoteTime;
            public float jumpBufferTime;
        }

        [System.Serializable]
        public class Sword
        {
            public int swordDamage;
            public float attackCooldown;
            public float knockbackMultiplier;
        }


        [System.Serializable]
        public class Gun
        {
            public bool unlimitedAmmo;
            public float bulletSpeed;
            public double shootingDelay;
            public float knockbackMultiplier;
            public double reloadTime;
        }


    }
}