using Assets.Scripts.Combat;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> weapons;
    [SerializeField] private int currentWeaponIndex;
    private Controller controller;

    private void Awake()
    {
        controller = GetComponent<Controller>();
        if (TryGetComponent(out Gun gun))
        {
            weapons.Add(gun);
        }
        if (TryGetComponent(out Sword sword))
        {
            weapons.Add(sword);
        }
    }

    private void Start()
    {
        // Ensure only one weapon is active at start
        foreach (MonoBehaviour weapon in weapons)
        {
            weapon.enabled = false;
        }
        weapons[0].enabled = true;
    }

    private void Update()
    {
        // Swap weapons when "Q" key is pressed
        if (controller.input.RetrieveSwapWeaponInput())
        {
            SwapWeapons();
        }
    }

    private void SwapWeapons()
    {
        Debug.Log("Switching weapon");
        weapons[currentWeaponIndex].enabled = false;
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        weapons[currentWeaponIndex].enabled = true;
        Debug.Log(weapons[currentWeaponIndex]);
    }
}