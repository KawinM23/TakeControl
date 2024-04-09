using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour, InputController
{
    public virtual float GetHorizontalMovement()
    {
        return 0f;
    }

    public virtual float GetVerticalMovement()
    {
        return 0f;
    }
    public virtual bool IsJumpPressed()
    {
        return false;
    }
    public virtual bool IsJumpHeld()
    {
        return false;
    }

    public virtual Vector2? GetAttackDirection()
    {
        return null;
    }
    public virtual Vector2? GetContinuedAttackDirection()
    {
        return null;
    }
    public virtual bool IsReloadPressed()
    {
        return false;
    }

    public virtual bool IsDashPressed()
    {
        return false;
    }
    public virtual bool IsSwapWeaponPressed()
    {
        return false;
    }

    public virtual Vector2? GetHackInput()
    {
        return null;
    }
}
