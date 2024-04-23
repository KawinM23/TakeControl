using Assets.Scripts.Combat;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<BaseWeapon> _weapons;
    [SerializeField] private int _currentWeaponIndex;
    private Controller _controller;

    private void Awake()
    {
        _controller = GetComponent<Controller>();
    }

    private void Start()
    {
        InitilizeWeapon();
    }

    private void InitilizeWeapon()
    {
        _weapons = new List<BaseWeapon>();
        if (TryGetComponent(out Gun gun))
        {
            _weapons.Add(gun);
        }
        if (TryGetComponent(out Sword sword))
        {
            _weapons.Add(sword);
        }

        // Ensure only one weapon is active at start
        foreach (BaseWeapon weapon in _weapons)
        {
            weapon.enabled = false;
        }
        _weapons[0].enabled = true;
    }

    private void Update()
    {
        // Swap weapons when "Q" key is pressed
        if (_controller.Input.IsSwapWeaponPressed())
        {
            SwapWeapons();
        }
    }

    private void SwapWeapons()
    {
        // Debug.Log("Switching weapon");
        _weapons[_currentWeaponIndex].enabled = false;
        _currentWeaponIndex = (_currentWeaponIndex + 1) % _weapons.Count;
        _weapons[_currentWeaponIndex].enabled = true;
        EquipmentUIManager.Instance.Select(_weapons[_currentWeaponIndex]);
        // Debug.Log(_weapons[_currentWeaponIndex]);
    }

    public BaseWeapon? CurrentWeapon()
    {
        if (_weapons.Count != 0)
        {
            return _weapons[_currentWeaponIndex];
        }
        else
        {
            return null;
        }

    }
}