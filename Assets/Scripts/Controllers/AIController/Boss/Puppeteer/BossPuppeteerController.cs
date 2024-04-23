using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Effect;
using UnityEngine;

#nullable enable annotations
public class BossPuppeteerController : AIController, InputController
{
    [SerializeField] private LayerMask _layerMask;

    // platforms to activate and deactivate + kills to enable platform
    [Header("Platforms")]
    [SerializeField] private GameObject _platforms;
    [SerializeField] private int _killsToEnablePlatform = 5;
    private Coroutine? _disablePlatformCountdown = null; // save coroutine as a field to cancel it anytime

    // These are spawners that this boss can control and should change spawning behavior in each phase
    [Header("Spawners")]
    [SerializeField] private GameObject _spawnerLeft;
    [SerializeField] private GameObject _spawnerRight;

    [SerializeField] private GameObject _spawnerSpecialLeft;
    [SerializeField] private GameObject _spawnerSpecialRight;

    // Laser Shooters
    [Header("Laser Shooters")]
    [SerializeField] private GameObject _lasersBottom;
    [SerializeField] private GameObject _lasersTop;

    // Health values realtime fetched from Health component
    [Header("Health")]
    private int _prev_health;
    private int _current_health;
    [Header("Gun")]
    private Gun _gun;


    void Awake()
    {
        _gun = GetComponent<Gun>();
    }

    void Start()
    {
        BossManager.Instance.ResetKillCount();
    }

    enum BossPhase
    {
        PHASE_1, // normal
        PHASE_2, // sniper knockback
        PHASE_3, // burst fire
        PHASE_4 // fall to ground, spawn turrets, burst fire but faster + more ammo
    }
    private BossPhase _phase = BossPhase.PHASE_1;
    private BossPhase? _prev_phase = null;
    private Coroutine? _phase_coroutine = null;


    // TODO: proper abstraction later (sealed class + pattern match?)
    // detect transition via functional callback?
    // pass common item (as struct)?
    enum State
    {
        INITAL,
        IDLE,
        SHOOTING,
        RELOADING
    }
    private State _state = State.IDLE;
    private State _prev_state = State.INITAL;
    private Coroutine? _state_coroutine = null;

#nullable enable

    void FixedUpdate()
    {
        UpdateShootingState();
        UpdateBossPhase();
        UpdateHealth();

        // if 3 enemies killed, enable platform until boss is hit
        if (!_platforms.activeInHierarchy && BossManager.Instance.GetEnemyKillCount() >= _killsToEnablePlatform) // todo: use variable
        {
            _platforms.SetActive(true);
            // disable platform after a certain amount of time
            Coroutine c = StartCoroutine(DisablePlatformsCountDown(15));
            _disablePlatformCountdown = c;
        }
    }

    void UpdateShootingState()
    {
        // if state unchanged, nothing to do
        if (_prev_state == _state)
        {
            return;
        }

        // if state CHANGED, stop current coroutine
        if (_state_coroutine != null)
        {
            Coroutine c = _state_coroutine;
            _state_coroutine = null;
            StopCoroutine(c);
            return;
        }

        // if state CHANGED, start new coroutine depending on current state
        _state_coroutine = _state switch
        {
            State.IDLE => StartCoroutine(IdleState()),
            State.SHOOTING => StartCoroutine(ShootingState()),
            State.RELOADING => StartCoroutine(ReloadingState()),
            _ => throw new System.Exception("Invalid state")
        };

        // Update previous state to check for state changes in the future
        _prev_state = _state;
    }

    void UpdateBossPhase()
    {
        // if phase unchanged, nothing to do
        if (_prev_phase == _phase)
        {
            return;
        }

        // if phase CHANGED, stop current coroutine
        if (_phase_coroutine != null)
        {
            Coroutine c = _phase_coroutine;
            _phase_coroutine = null;
            StopCoroutine(c);
            return;
        }

        // if phase CHANGED, start new coroutine depending on current phase
        _phase_coroutine = _phase switch
        {
            BossPhase.PHASE_1 => StartCoroutine(Phase1()),
            BossPhase.PHASE_2 => StartCoroutine(Phase2()),
            BossPhase.PHASE_3 => StartCoroutine(Phase3()),
            BossPhase.PHASE_4 => StartCoroutine(Phase4()),

            _ => throw new System.Exception("Invalid phase")
        };

        // Update previous phase to check for phase changes in the future
        _prev_phase = _phase;
    }

    void UpdateHealth()
    {
        // no disable platform in phase 4
        if (_phase == BossPhase.PHASE_4)
        {
            return;
        }

        if (TryGetComponent<Health>(out Health health))
        {
            _current_health = health.GetCurrentHealth();
            // If boss takes damage, disable platforms
            if (_current_health < _prev_health)
            {
                _platforms.SetActive(false);
                if (_disablePlatformCountdown != null)
                {
                    StopCoroutine(_disablePlatformCountdown);
                    _disablePlatformCountdown = null;
                }
                BossManager.Instance.ResetKillCount();
            }
            _prev_health = _current_health;
        }
    }

    IEnumerator DisablePlatformsCountDown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _platforms.SetActive(false);
    }

    IEnumerator IdleState()
    {
        while (true)
        {
            var player = PlayerManager.Instance.Player;
            if (player == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }


            if (RaycastPlayer())
            {
                yield return new WaitForFixedUpdate();
                _state = State.SHOOTING;
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ShootingState()
    {
        while (true)
        {
            if (_gun.CurrentAmmo == 0)
            {
                yield return new WaitForFixedUpdate();
                _state = State.RELOADING;
                yield break;
            }

            if (!RaycastPlayer())
            {
                yield return new WaitForFixedUpdate();
                _state = State.IDLE;
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator ReloadingState()
    {
        while (_gun.CurrentAmmo == 0)
        {
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
        _state = State.IDLE;
        yield break;
    }

    IEnumerator Phase1()
    {
        Debug.Log("Phase 1");

        // disable special spawner
        _spawnerSpecialLeft.SetActive(false);
        _spawnerSpecialRight.SetActive(false);

        // set spawner left
        SpawnerController.SpawnerSettings spawnerLeftSettings = new SpawnerController.SpawnerSettings
        {
            delayedStart = 0f,
            spawnCountMax = 2,
            spawnCountMin = 1,
            spawnIntervalMax = 1f,
            spawnIntervalMin = 0f,
            waveCount = 99, // endless
            waveInterval = 5f,
            currentWave = 0,
            spawningGameObject = SpawnerController.SpawningGameObject.SWORD_CHARGER,
            maxSpawnObjects = 3
        };
        _spawnerLeft.GetComponent<SpawnerController>().SetSpawnerSettings(spawnerLeftSettings);

        // set spawner right
        SpawnerController.SpawnerSettings spawnerRightSettings = new SpawnerController.SpawnerSettings
        {
            delayedStart = 0f,
            spawnCountMax = 2,
            spawnCountMin = 1,
            spawnIntervalMax = 1f,
            spawnIntervalMin = 0f,
            waveCount = 99, // endless
            waveInterval = 5f,
            currentWave = 0,
            spawningGameObject = SpawnerController.SpawningGameObject.SWORD_CHARGER,
            maxSpawnObjects = 3
        };
        _spawnerRight.GetComponent<SpawnerController>().SetSpawnerSettings(spawnerRightSettings);

        // disable all platforms
        _platforms.SetActive(false);

        // hide all lasers
        _lasersBottom.SetActive(false);
        _lasersTop.SetActive(false);

        // wait until health is less than 50%
        Health health = GetComponent<Health>();
        if (health == null)
        {
            yield break;
        }
        while (health.GetCurrentHealth() > health.GetMaxHealth() / 2)
        {
            yield return new WaitForFixedUpdate();
        }
        // change to phase 2
        yield return new WaitForFixedUpdate();
        _phase = BossPhase.PHASE_2;
        yield break;
    }

    IEnumerator Phase2()
    {
        // keep spawner left settings

        // set spawner right
        // we only have sword charger for player to hack, so they will surely take damage from bombers
        // spawn bombers at a rate that player have chance to hack another sword charger to heal
        // bomber prefab currently has 30 damage, let's give player 10 seconds to hack another sword charger
        SpawnerController.SpawnerSettings spawnerRightSettings = new SpawnerController.SpawnerSettings
        {
            delayedStart = 0f,
            spawnCountMax = 3,
            spawnCountMin = 1,
            spawnIntervalMax = 2f,
            spawnIntervalMin = 0f,
            waveCount = 99, // endless
            waveInterval = 10f,
            currentWave = 0,
            spawningGameObject = SpawnerController.SpawningGameObject.BOMBER,
            maxSpawnObjects = 2
        };
        _spawnerRight.GetComponent<SpawnerController>().SetSpawnerSettings(spawnerRightSettings);

        // set gun stats
        // heavy knockback
        // shoot slowly
        if (TryGetComponent<Gun>(out Gun _gun))
        {
            _gun.MaxAmmo = 10;
            _gun.CurrentAmmo = 10;
            _gun.SetShootingDelay(3);
            _gun.SetKnockbackMultiplier(20);
            _gun.SetBulletSpeed(40);
        }

        // enable bottom laser
        _lasersBottom.SetActive(true);

        // recover to full hp
        Health health = GetComponent<Health>();
        if (health == null)
        {
            yield break;
        }
        health.ResetHealth();

        // wait until health is less than 50%
        while (health.GetCurrentHealth() > health.GetMaxHealth() / 2)
        {
            yield return new WaitForFixedUpdate();
        }

        // change to phase 3
        yield return new WaitForFixedUpdate();
        _phase = BossPhase.PHASE_3;
        yield break;
    }

    IEnumerator Phase3()
    {
        // set gun stats
        // shoot in bursts
        // slow reload
        if (TryGetComponent<Gun>(out Gun _gun))
        {
            _gun.MaxAmmo = 8;
            _gun.CurrentAmmo = 8;
            _gun.SetShootingDelay(0.2);
            _gun.SetKnockbackMultiplier(3);
            _gun.SetBulletSpeed(10);
            _gun.SetReloadTime(4);
        }

        // enable top lasers
        _lasersTop.SetActive(true);

        // recover to full hp
        Health health = GetComponent<Health>();
        if (health == null)
        {
            yield break;
        }
        health.ResetHealth();

        // wait until health is less than 50%
        while (health.GetCurrentHealth() > health.GetMaxHealth() / 2)
        {
            yield return new WaitForFixedUpdate();
        }

        // change to phase 3
        yield return new WaitForFixedUpdate();
        _phase = BossPhase.PHASE_4;
        yield break;
    }

    IEnumerator Phase4()
    {
        // spawn special turret
        SpawnerController.SpawnerSettings spawnerSpecialLeftSettings = new SpawnerController.SpawnerSettings
        {
            delayedStart = 0f,
            spawnCountMax = 1,
            spawnCountMin = 1,
            spawnIntervalMax = 2f,
            spawnIntervalMin = 1f,
            waveCount = 10, // spawn once
            waveInterval = 30f,
            currentWave = 0,
            spawningGameObject = SpawnerController.SpawningGameObject.BOSS_PUPPETEER_TURRET,
            maxSpawnObjects = 1
        };
        // enable special spawner
        _spawnerSpecialLeft.SetActive(true);
        _spawnerSpecialRight.SetActive(true);

        // set gun stats
        // shoot in bursts
        // slow reload
        // but on steroids
        if (TryGetComponent<Gun>(out Gun _gun))
        {
            _gun.MaxAmmo = 20;
            _gun.CurrentAmmo = 20;
            _gun.SetShootingDelay(0.1);
            _gun.SetKnockbackMultiplier(3);
            _gun.SetBulletSpeed(15);
            _gun.SetReloadTime(3);
        }

        // Permanently Enable Platforms
        _platforms.SetActive(true);

        // todo: make boss fall to ground
        // if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        // {
        //     rb.bodyType = RigidbodyType2D.Dynamic;
        //     rb.gravityScale = 1;
        //     ScreenShake.Shake(ScreenShake.ShakeType.ShootBigBullet);
        // }

        // todo continue here
        // Set Boss to high HP
        if (TryGetComponent<Health>(out Health health))
        {
            health.ResetHealthWithNewMaxHealth(1000);
        }

        _spawnerSpecialLeft.GetComponent<SpawnerController>().SetSpawnerSettings(spawnerSpecialLeftSettings);
        yield break;
    }


    public override Vector2? GetAttackDirection()
    {
        return _state == State.SHOOTING ? PlayerManager.Instance.Player.transform.position : null;
    }

    public override bool IsReloadPressed()
    {
        return _state == State.RELOADING;
    }

    public override bool IsJumpPressed()
    {
        return _state == State.RELOADING;
    }

    private bool RaycastPlayer()
    {
        var player = PlayerManager.Instance.Player;
        var cast = Physics2D.Linecast(transform.position, player.transform.position, _layerMask);
        if (cast && cast.transform.gameObject == gameObject)
        {
            Debug.LogWarning("Linecast hit itself");
        }
        return !cast || cast.transform.gameObject == player;
    }
}

