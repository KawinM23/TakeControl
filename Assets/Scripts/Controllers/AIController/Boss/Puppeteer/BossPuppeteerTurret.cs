using System.Collections;
using Assets.Scripts.Combat;
using UnityEngine;

#nullable enable annotations
public class BossPuppeteerTurretController : AIController, InputController
{
    [SerializeField] private LayerMask _layerMask;
    private Gun _gun;

    // After running out of ammo, spawn one sword charger with instant hackable health
    [Header("Sword Charger")]
    [SerializeField] private GameObject _swordChargerPrefab;
    private bool _spawnedSwordCharger = false;

    void Awake()
    {
        _gun = GetComponent<Gun>();
        _spawnedSwordCharger = false; // repeat here so that no one accidentally set it to true
    }

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
        UpdateState();
        // if gun ammo is 0, spawn sword charger (must be when player is in control)
        if (!_spawnedSwordCharger && _gun.CurrentAmmo == 0 && tag.Equals("Player"))
        {
            Debug.Log("ammo spawned" + _gun.CurrentAmmo + " " + _spawnedSwordCharger);
            SpawnSwordCharger();
        }

    }

    void UpdateState()
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

    void SpawnSwordCharger()
    {
        if (_spawnedSwordCharger)
        {
            return;
        }
        _spawnedSwordCharger = true;
        Vector3 position = transform.position;
        // spawn higher so that it doesn't collide with the turret
        position.y += 1;
        var swordCharger = Instantiate(_swordChargerPrefab, position, Quaternion.identity);
        Health health = swordCharger.GetComponent<Health>();
        health.SetHackableHealth(health.GetMaxHealth());
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

    public void SetGunAmmo(int ammo)
    {
        _gun.CurrentAmmo = (uint)ammo;
    }
}

