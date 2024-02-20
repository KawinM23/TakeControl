using Assets.Scripts.Combat;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Gun gun;
    public Sword sword;
    private Controller controller;

    private void Awake()
    {
        controller = GetComponent<Controller>();
    }

    private void Start()
    {
        // Ensure only one weapon is active at start
        if (gun)
        {
            gun.enabled = false;
        }

        if (sword)
        {
            sword.enabled = true;
        }
    }

    private void Update()
    {
        // Swap weapons when "Q" key is pressed
        if (controller.input.RetrieveSwapWeaponInput())
        {
            Debug.Log("Switching weapon");
            SwapWeapons();
            Debug.Log(sword.enabled);
        }
    }

    private void SwapWeapons()
    {
        // Toggle weapon activation
        if (gun && sword)
        {
            gun.enabled = !gun.enabled;
            sword.enabled = !gun.enabled;
        }
    }
}