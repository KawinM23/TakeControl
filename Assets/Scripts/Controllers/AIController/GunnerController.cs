using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using UnityEngine;

#nullable enable annotations

[RequireComponent(typeof(Gun), typeof(FieldOfView))]
public class GunnerController : AIController, InputController
{
    [SerializeField] private float _patrolRadius = 5f;
    [Tooltip("Preferred distance from player")]
    public float preferredDistance = 5f;

    [Tooltip("Chance to jump per second (on average)")]
    public float _jumpChance = 0.2f;

    [Header("Movement speed")]
    [Tooltip("Movement speed while patrolling")]
    [SerializeField] private float _idleSpeed = 0.5f;
    [Tooltip("Movement speed while shooting")]
    [SerializeField] private float _attackingSpeed = 0.8f;
    [Tooltip("Movement speed while reloading")]
    [SerializeField] private float _reloadingSpeed = 0.6f;



    private Vector2 _home;
    private Gun _gun;
    private FieldOfView _fov;
    private BoxCollider2D _collider;



    void Awake()
    {
        _gun = GetComponent<Gun>();
        _fov = GetComponent<FieldOfView>();
        _collider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        _home = transform.position;
    }

    enum State
    {
        INITAL,
        IDLE,
        ATTACKING,
        RELOADING
    }
    private State _state = State.IDLE;
    private State _prev_state = State.INITAL;
    private Coroutine? _state_coroutine = null;

#nullable enable

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
            State.IDLE => StartCoroutine(IdleState()),
            State.ATTACKING => StartCoroutine(AttackingState()),
            State.RELOADING => StartCoroutine(ReloadingState()),
            _ => throw new Exception("Invalid state")
        };

        // Update previous state to check for state changes in the future
        _prev_state = _state;
    }

    private float _idleMovement;
    [SerializeField] private float lastMoveTime;
    IEnumerator IdleState()
    {
        var player = PlayerManager.Instance.Player;
        _idleMovement = player.transform.position.x > transform.position.x ? _idleSpeed : -_idleSpeed;
        lastMoveTime = Time.time;
        Vector2 lastPos = transform.position;

        while (true)
        {
            // Check for player
            var canSeePlayer = _fov.FieldOfViewCheck();
            if (canSeePlayer)
            {
                yield return new WaitForFixedUpdate();
                _state = State.ATTACKING;
                yield break;
            }

            // Track last move time
            if (Vector2.SqrMagnitude(lastPos - (Vector2)transform.position) >= 0.01f)
            {
                lastMoveTime = Time.time;
                lastPos = transform.position;
            }

            if (Mathf.Approximately(_patrolRadius, 0f))
            {
                _idleMovement = 0;
            }
            else
            {
                // Patrol around home
                var distFromHome = Vector2.Distance(transform.position, _home);
                if (distFromHome >= _patrolRadius || (Time.time - lastMoveTime > 0.2f) || IsHoleBelow())
                {
                    _idleMovement = _home.x > transform.position.x ? _idleSpeed : -_idleSpeed;
                }
            }
;
            yield return new WaitForFixedUpdate();
        }
    }

    private float _atackingMovement;
    IEnumerator AttackingState()
    {
        var player = PlayerManager.Instance.Player;

        while (true)
        {
            if (_gun.CurrentAmmo == 0)
            {
                yield return new WaitForFixedUpdate();
                _state = State.RELOADING;
                yield break;
            }

            _atackingMovement = CalculateAttackMovement(_attackingSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator ReloadingState()
    {
        while (true)
        {
            if (_gun.CurrentAmmo != 0)
            {
                yield return new WaitForFixedUpdate();
                _state = _fov.FieldOfViewCheck() ? State.ATTACKING : State.IDLE;
                yield break;
            }

            _atackingMovement = CalculateAttackMovement(_reloadingSpeed);

            yield return new WaitForFixedUpdate();
        }
    }

    private float CalculateAttackMovement(float maxSpeed)
    {
        var player = PlayerManager.Instance.Player;
        var dist = player.transform.position.x - transform.position.x;

        // if going to fall, don't
        var wouldFall = (dist > 0 && IsHoleBelowRight()) || (dist < 0 && IsHoleBelowLeft());
        if (wouldFall)
        {
            return 0;
        }
        var mov = Mathf.Sign(dist) * preferredDistance;
        mov = Mathf.Clamp(mov, -maxSpeed, maxSpeed);
        return mov;
    }

    private bool IsHoleBelow()
    {
        var mask = _fov.obstructionMask; // ???
        var pos = (Vector2)transform.position + new Vector2(0, -_collider.bounds.extents.y);
        var hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, mask);
        return !hit;
    }

    private bool IsHoleBelowRight()
    {
        var mask = _fov.obstructionMask; // ???
        var pos = (Vector2)transform.position + new Vector2(_collider.bounds.extents.x, -_collider.bounds.extents.y);
        var hit = Physics2D.Raycast(pos, Vector2.down, 1f, mask);
        return !hit;
    }

    private bool IsHoleBelowLeft()
    {
        var mask = _fov.obstructionMask; // ???
        var pos = (Vector2)transform.position + new Vector2(-_collider.bounds.extents.x, -_collider.bounds.extents.y);
        var hit = Physics2D.Raycast(pos, Vector2.down, 1f, mask);
        return !hit;
    }


    public override float GetHorizontalMovement()
    {
        var a = _state switch
        {
            State.IDLE => _idleMovement,
            State.ATTACKING or State.RELOADING => _atackingMovement,
            _ => base.GetHorizontalMovement()
        };
        return a;
    }

    public override bool IsJumpPressed()
    {
        return _state != State.IDLE && UnityEngine.Random.value < _jumpChance * Time.smoothDeltaTime;
    }

    public override Vector2? GetAttackDirection()
    {
        return _state == State.ATTACKING ? PlayerManager.Instance.Player.transform.position : null;
    }
}

