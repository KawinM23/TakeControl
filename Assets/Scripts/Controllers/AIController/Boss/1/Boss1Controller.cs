using System.Collections.Generic;
using UnityEngine;

// This is a template file for AIController
// IDLE util found player -> SHOOT -> RELOAD (while jumping) -> IDLE

#nullable enable annotations
public class Boss1Controller : Boss1BaseController
{
    [SerializeField] private List<Boss1ComponentController> components;
    [SerializeField] private double _baseHealPercent;

    enum Phase
    {
        NORMAL,
        Boost1,
        Boost2
    }
    private Phase _phase = Phase.NORMAL;

    protected override void OnUpdate()
    {
        if (IsDestroy())
        {
            foreach (Boss1ComponentController comp in components)
            {
                comp.GetHealth().SetMortality(true);
            }
            _health.SetMortality(true);
            return;
        }

        if (_healTimer >= 0)
        {
            _healTimer -= Time.deltaTime;
        }
        if (_healTimer <= 0)
        {
            _health.HealPercent(_baseHealPercent * (1 + (components.Count - CountDestroyComponents())));
            _healTimer = _healDelay;
        }

        if (_phase == Phase.NORMAL && CountDestroyComponents() > 0)
        {
            _phase = Phase.Boost1;
            foreach (Boss1ComponentController comp in components)
            {
                if (!comp.IsDestroy())
                {
                    comp.SetHealPercent(comp.GetHealPercent() * 1.5);
                    comp.GetGun().SetBulletSpeed(comp.GetGun().GetBulletSpeed() * 2);
                    comp.GetGun().SetShootingDelay(comp.GetGun().GetShootingDelay() / 2);
                }
            }
            _gun.SetShootingDelay(_gun.GetShootingDelay() / 2);
        }
        else if (_phase == Phase.Boost1 && components.Count == CountDestroyComponents())
        {
            _phase = Phase.Boost2;
            _gun.SetShootingDelay(_gun.GetShootingDelay() / 2);
        }
    }

    private int CountDestroyComponents()
    {
        int destroyComponents = 0;

        foreach (Boss1ComponentController comp in components)
        {
            if (comp.IsDestroy()) destroyComponents += 1;
        }
        return destroyComponents;
    }
}

