using System.Collections.Generic;
using UnityEngine;

// This is a template file for AIController
// IDLE util found player -> SHOOT -> RELOAD (while jumping) -> IDLE

#nullable enable annotations
public class Boss1Controller : Boss1BaseController
{
    [SerializeField] private List<Boss1ComponentController> components;
    [SerializeField] private double _baseHealPercent;
    [SerializeField] private LaserShooter beginLaser;
    [SerializeField] private LaserShooter endLaser;
    private List<LaserShooter> allLaser;
    private int laserPatternCounter = 0;

    enum Phase
    {
        NORMAL,
        BOOST1,
        BOOST2
    }

    enum LaserPattern
    {
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT,
        MIDDLE_TO_REAR,
        REAR_TO_MIDDLE
    }
    private Phase _phase = Phase.NORMAL;

    protected override void Awake()
    {
        base.Awake();
        allLaser = new List<LaserShooter>();
        beginLaser.SetActive(false);
        beginLaser.SetAutoDeactive(true);
        beginLaser.SetDuration(0.2f);
        endLaser.SetActive(false);
        endLaser.SetAutoDeactive(true);
        endLaser.SetDuration(0.2f);
        if (beginLaser.transform.position.x <= endLaser.transform.position.x)
        {
            allLaser.Add(beginLaser);
            for (float i = beginLaser.transform.position.x + beginLaser.transform.localScale.x; i < endLaser.transform.position.x; i += beginLaser.transform.localScale.x)
            {
                LaserShooter newLaser = Instantiate(beginLaser, new Vector3(i, beginLaser.transform.position.y, 0f), Quaternion.identity);
                newLaser.SetAutoDeactive(true);
                newLaser.SetDuration(0.2f);
                allLaser.Add(newLaser);
            }
            allLaser.Add(endLaser);
        }
    }

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
            _phase = Phase.BOOST1;
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
        else if (_phase == Phase.BOOST1 && components.Count == CountDestroyComponents())
        {
            _phase = Phase.BOOST2;
            _gun.SetShootingDelay(_gun.GetShootingDelay() / 2);
        }
        if (_phase == Phase.NORMAL && IsAllLaserDeactivate())
        {
            ActivateLaserPattern((LaserPattern)(laserPatternCounter % 4));
            laserPatternCounter++;
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

    private bool IsAllLaserDeactivate()
    {
        foreach (LaserShooter laser in allLaser)
        {
            if (laser.IsActive()) return false;
        }
        return true;
    }

    private void ActivateLaserPattern(LaserPattern pattern)
    {
        if (IsAllLaserDeactivate())
        {
            switch (pattern)
            {
                case LaserPattern.LEFT_TO_RIGHT:
                    for (int i = 0; i < allLaser.Count; i++)
                    {
                        float value = ((float)i / 10) + 1;
                        allLaser[i].SetTimer(value);
                        allLaser[i].SetDelay(3);
                        allLaser[i].SetActive(true);
                    }
                    break;
                case LaserPattern.RIGHT_TO_LEFT:
                    for (int i = 0; i < allLaser.Count; i++)
                    {
                        float value = (((float)allLaser.Count - i - 1) / 10) + 1;
                        allLaser[i].SetTimer(value);
                        allLaser[i].SetDelay(1);
                        allLaser[i].SetActive(true);
                    }
                    break;
                case LaserPattern.MIDDLE_TO_REAR:
                    for (int i = 0; i < (allLaser.Count + 1) / 2; i++)
                    {
                        float value = (((float)((allLaser.Count + 1) / 2) - i - 1) / 10) + 1;
                        allLaser[i].SetTimer(value);
                        allLaser[i].SetDelay(1);
                        allLaser[i].SetActive(true);
                        if (i == allLaser.Count - i - 1) break;
                        allLaser[allLaser.Count - i - 1].SetTimer(value);
                        allLaser[allLaser.Count - i - 1].SetDelay(1);
                        allLaser[allLaser.Count - i - 1].SetActive(true);
                    }
                    break;
                case LaserPattern.REAR_TO_MIDDLE:
                    for (int i = 0; i < (allLaser.Count + 1) / 2; i++)
                    {
                        float value = (((float)i) / 10) + 1;
                        allLaser[i].SetTimer(value);
                        allLaser[i].SetDelay(1);
                        allLaser[i].SetActive(true);
                        if (i == allLaser.Count - i - 1) break;
                        allLaser[allLaser.Count - i - 1].SetTimer(value);
                        allLaser[allLaser.Count - i - 1].SetDelay(1);
                        allLaser[allLaser.Count - i - 1].SetActive(true);
                    }
                    break;
            }
        }
    }
}

