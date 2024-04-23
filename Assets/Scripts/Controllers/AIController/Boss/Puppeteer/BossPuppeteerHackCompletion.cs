using UnityEngine;
using Assets.Scripts.Combat;

public class BossPuppeteerHackCompletion : HackCompletion
{
    public override void OnHackSuccess()
    {
        return;
    }
    public override void OnHackFail()
    {
        return;
    }
    public override void OnComboHackSuccess(int comboCount)
    {
        // Set health to ridiculously a lot
        if (TryGetComponent(out Health health))
        {
            health.ResetHealthWithNewMaxHealth(999999);
            health.SetHackableHealth(1); // dont let player hack it again
        }
        else
        {
            Debug.LogError("BossPuppeteerHackCompletion: OnComboHackSuccess: Health component not found");
        }

        // set gun ammo to comboCount
        if (TryGetComponent(out Gun gun))
        {
            gun.CurrentAmmo = (uint)comboCount;
        }
        else
        {
            Debug.LogError("BossPuppeteerHackCompletion: OnComboHackSuccess: Gun component not found");
        }
        return;
    }
    public override void OnComboHackFail(int comboCount)
    {
        return;
    }
}