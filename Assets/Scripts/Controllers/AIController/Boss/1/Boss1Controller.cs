using System.Collections.Generic;
using UnityEngine;

// This is a template file for AIController
// IDLE util found player -> SHOOT -> RELOAD (while jumping) -> IDLE

#nullable enable annotations
public class Boss1Controller : Boss1BaseController
{
    [SerializeField] private List<Boss1ComponentController> _components;
    [SerializeField] private double _baseHealPercent;
    [SerializeField] private LaserShooter _beginLaser;
    [SerializeField] private LaserShooter _endLaser;
    [SerializeField] private GameObject _barrier;
    [SerializeField] private GameObject _door;
    private List<LaserShooter> _allLaser;
    private int _laserPatternCounter = 0;
    private float _phaseDelayTimer = 10;

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
        _allLaser = new List<LaserShooter>();
        _beginLaser.SetActive(false);
        _beginLaser.SetAutoDeactive(true);
        _beginLaser.SetDuration(0.2f);
        _endLaser.SetActive(false);
        _endLaser.SetAutoDeactive(true);
        _endLaser.SetDuration(0.2f);
        if (_beginLaser.transform.position.x <= _endLaser.transform.position.x)
        {
            _allLaser.Add(_beginLaser);
            for (float i = _beginLaser.transform.position.x + _beginLaser.transform.localScale.x; i < _endLaser.transform.position.x; i += _beginLaser.transform.localScale.x)
            {
                LaserShooter newLaser = Instantiate(_beginLaser, new Vector3(i, _beginLaser.transform.position.y, 0f), Quaternion.identity);
                newLaser.SetAutoDeactive(true);
                newLaser.SetDuration(0.2f);
                _allLaser.Add(newLaser);
            }
            _allLaser.Add(_endLaser);
        }
        _door.SetActive(false);
    }

    protected override void OnUpdate()
    {
        if (IsDestroy())
        {
            foreach (Boss1ComponentController comp in _components)
            {
                comp.GetHealth().SetMortality(true);
            }
            _health.SetMortality(true);
            foreach (LaserShooter laser in _allLaser)
            {
                laser.SetActive(false);
            }
            SoundManager.Instance.StopBGM();
            _door.SetActive(false);
            return;
        }

        if (PlayerManager.Instance.Player.transform.position.x >= (_door.transform.position.x + PlayerManager.Instance.Player.transform.localScale.x) && PlayerManager.Instance.Player.transform.position.y <= (_door.transform.position.y + _door.transform.localScale.y))
        {
            _door.SetActive(true);
        }
        if (_healTimer >= 0)
        {
            _healTimer -= Time.deltaTime;
        }
        if (_healTimer <= 0)
        {
            _health.HealPercent(_baseHealPercent * (1 + (_components.Count - CountDestroyComponents())));
            _healTimer = _healDelay;
        }

        if (_phase == Phase.NORMAL && CountDestroyComponents() > 0)
        {
            _phase = Phase.BOOST1;
            foreach (Boss1ComponentController comp in _components)
            {
                if (!comp.IsDestroy())
                {
                    comp.SetHealPercent(comp.GetHealPercent() * 1.5);
                    comp.GetGun().SetBulletSpeed(comp.GetGun().GetBulletSpeed() * 1.5f);
                    comp.GetGun().SetShootingDelay(comp.GetGun().GetShootingDelay() / 1.5);
                }
            }
            _gun.SetShootingDelay(_gun.GetShootingDelay() / 1.5);
        }
        else if (_phase == Phase.BOOST1 && _components.Count == CountDestroyComponents())
        {
            _phase = Phase.BOOST2;
            _gun.SetShootingDelay(_gun.GetShootingDelay() / 1.5);
            Destroy(_barrier);
        }
        if (_phase == Phase.BOOST2 && IsAllLaserDeactivate())
        {
            if (_phaseDelayTimer > 0)
            {
                _phaseDelayTimer -= Time.deltaTime;
                if (_phaseDelayTimer <= 0) SoundManager.Instance.PlayBGM("CMB Boss1Fight");
            }
            else
            {
                ActivateLaserPattern((LaserPattern)(_laserPatternCounter));
                _laserPatternCounter = (_laserPatternCounter + 1) % 4;
            }
        }
    }

    private int CountDestroyComponents()
    {
        int destroyComponents = 0;

        foreach (Boss1ComponentController comp in _components)
        {
            if (comp.IsDestroy()) destroyComponents += 1;
        }
        return destroyComponents;
    }

    private bool IsAllLaserDeactivate()
    {
        foreach (LaserShooter laser in _allLaser)
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
                    for (int i = 0; i < _allLaser.Count; i++)
                    {
                        float value = ((float)i / 10) + 1;
                        _allLaser[i].SetTimer(value);
                        _allLaser[i].SetDelay(3);
                        _allLaser[i].SetActive(true);
                    }
                    break;
                case LaserPattern.RIGHT_TO_LEFT:
                    for (int i = 0; i < _allLaser.Count; i++)
                    {
                        float value = (((float)_allLaser.Count - i - 1) / 10) + 1;
                        _allLaser[i].SetTimer(value);
                        _allLaser[i].SetDelay(1);
                        _allLaser[i].SetActive(true);
                    }
                    break;
                case LaserPattern.MIDDLE_TO_REAR:
                    for (int i = 0; i < (_allLaser.Count + 1) / 2; i++)
                    {
                        float value = (((float)((_allLaser.Count + 1) / 2) - i - 1) / 10) + 1;
                        _allLaser[i].SetTimer(value);
                        _allLaser[i].SetDelay(1);
                        _allLaser[i].SetActive(true);
                        if (i == _allLaser.Count - i - 1) break;
                        _allLaser[_allLaser.Count - i - 1].SetTimer(value);
                        _allLaser[_allLaser.Count - i - 1].SetDelay(1);
                        _allLaser[_allLaser.Count - i - 1].SetActive(true);
                    }
                    break;
                case LaserPattern.REAR_TO_MIDDLE:
                    for (int i = 0; i < (_allLaser.Count + 1) / 2; i++)
                    {
                        float value = (((float)i) / 10) + 1;
                        _allLaser[i].SetTimer(value);
                        _allLaser[i].SetDelay(1);
                        _allLaser[i].SetActive(true);
                        if (i == _allLaser.Count - i - 1) break;
                        _allLaser[_allLaser.Count - i - 1].SetTimer(value);
                        _allLaser[_allLaser.Count - i - 1].SetDelay(1);
                        _allLaser[_allLaser.Count - i - 1].SetActive(true);
                    }
                    break;
            }
        }
    }
}

