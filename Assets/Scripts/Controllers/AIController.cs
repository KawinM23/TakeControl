using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIController", menuName = "InputController/AIController")]
public class AIController : InputController
{
    public override float GetHorizontalMovement()
    {
        return 0f;
    }
    public override float GetVerticalMovement()
    {
        return 0f;
    }
    public override bool IsJumpPressed()
    {
        return false;
    }
    public override bool IsJumpHeld()
    {
        return false;
    }

    public override Vector2? GetAttackDirection()
    {
        return null;
    }
    public override Vector2? GetContinuedAttackDirection()
    {
        return null;
    }
    public override bool IsReloadPressed()
    {
        return false;
    }

    public override bool IsDashPressed()
    {
        return false;
    }
    public override bool IsSwapWeaponPressed()
    {
        return false;
    }

    public override Vector2? GetHackInput()
    {
        return null;
    }
}
