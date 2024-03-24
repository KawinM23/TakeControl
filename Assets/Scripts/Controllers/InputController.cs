using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    // Movement
    public abstract float GetHorizontalMovement();
    public abstract float GetVerticalMovement();
    public abstract bool IsJumpPressed();
    public abstract bool IsJumpHeld();
    public abstract bool IsDashPressed();

    // Combat
    public abstract Vector2? GetAttackDirection();
    public abstract Vector2? GetContinuedAttackDirection();
    public abstract bool IsReloadPressed();
    public abstract bool IsSwapWeaponPressed();

    // Hack
    public abstract Vector2? GetHackInput();
}
