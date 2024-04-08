using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputController
{
    // Movement
    float GetHorizontalMovement();
    float GetVerticalMovement();
    bool IsJumpPressed();
    bool IsJumpHeld();
    bool IsDashPressed();

    // Combat
    Vector2? GetAttackDirection();
    Vector2? GetContinuedAttackDirection();
    bool IsReloadPressed();
    bool IsSwapWeaponPressed();

    // Hack
    Vector2? GetHackInput();
}
