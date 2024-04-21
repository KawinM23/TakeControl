using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using UnityEngine;

// This is a template file for AIController
// IDLE util found player -> SHOOT -> RELOAD (while jumping) -> IDLE

#nullable enable annotations
public class MenuDemoController : AIController, InputController
{
    [SerializeField] private Vector3 _shootingTarget;
    private Gun _gun;


    void Awake()
    {
        _gun = GetComponent<Gun>();
    }

    // TODO: proper abstraction later (sealed class + pattern match?)
    // detect transition via functional callback?
    // pass common item (as struct)?
    enum State
    {
        INITAL,
      
        SHOOTING,
        RELOADING
    }
    private State _state = State.SHOOTING;
    private State _prev_state = State.INITAL;
    private Coroutine? _state_coroutine = null;
    private Vector2 _home;

#nullable enable
    void Start()
    {
        _home = transform.position;
    }

    void FixedUpdate()
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
           
            State.SHOOTING => StartCoroutine(ShootingState()),
            State.RELOADING => StartCoroutine(ReloadingState()),
            _ => throw new System.Exception("Invalid state")
        };

        // Update previous state to check for state changes in the future
        _prev_state = _state;
    }


    private float _idleMovement;
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
            var distFromHome = Vector2.Distance(transform.position, _home);
            if (distFromHome <= 5f)
            {
                _idleMovement = Random.Range(0.5f, 1f);
                _idleMovement *= Random.value < 0.5 ? 1 : -1;
            }
            else
            {
                _idleMovement = Mathf.Clamp(_home.x - transform.position.x, -0.5f, 0.5f);
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
        _state = State.SHOOTING;
        yield break;
    }


    public override Vector2? GetAttackDirection()
    {
        return _state == State.SHOOTING ? _shootingTarget : null;
    }

    public override float GetHorizontalMovement()
    {
        return _state switch
        {
            State.SHOOTING => _idleMovement,
            State.RELOADING => 0,
            _ => base.GetHorizontalMovement()
        };
    }

    public override bool IsReloadPressed()
    {
        return _state == State.RELOADING;
    }

    // will jump when state is reloading
    public override bool IsJumpPressed()
    {

        if (_state == State.RELOADING)
        {
            return true; // If reloading, always return true
        }

        else
        {
      
            float randomValue = Random.value;

           
            if (randomValue <= 0.0025f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public override bool IsDashPressed()
    {


        return true;
    }
}

