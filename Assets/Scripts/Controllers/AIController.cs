using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, InputController
{
    public float GetHorizontalMovement()
    {
        return 0f;
    }

    public float GetVerticalMovement()
    {
        return 0f;
    }
    public bool IsJumpPressed()
    {
        return false;
    }
    public bool IsJumpHeld()
    {
        return false;
    }

    public Vector2? GetAttackDirection()
    {
        return null;
    }
    public Vector2? GetContinuedAttackDirection()
    {
        return null;
    }
    public bool IsReloadPressed()
    {
        return false;
    }

    public bool IsDashPressed()
    {
        return false;
    }
    public bool IsSwapWeaponPressed()
    {
        return false;
    }

    public Vector2? GetHackInput()
    {
        return null;
    }
}
