using System;
using Assets.Scripts.Combat;
using UnityEngine;

#nullable enable annotations
public class Boss1ComponentController : Boss1BaseController
{
    [SerializeField] private double _healPercent;

    public double GetHealDelay()
    {
        return _healDelay;
    }

    public void SetHealDelay(double healDelay)
    {
        _healDelay = Math.Max(0, healDelay);
    }

    public double GetHealPercent()
    {
        return _healPercent;
    }

    public void SetHealPercent(double healPercent)
    {
        _healPercent = Math.Max(0, healPercent);
    }

    public Gun GetGun()
    {
        return _gun;
    }

    public Health GetHealth()
    {
        return _health;
    }

    protected override void OnUpdate()
    {
        if (IsDestroy()) return;

        if (_healTimer >= 0)
        {
            _healTimer -= Time.deltaTime;
        }
        if (_healTimer <= 0)
        {
            _health.HealPercent(_healPercent);
            _healTimer = _healDelay;
        }
    }
}

