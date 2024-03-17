using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    public override float GetHorizontalMovement()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public override float GetVerticalMovement()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public override bool IsJumpPressed()
    {
        return Input.GetButtonDown("Jump");
    }
    public override bool IsJumpHeld()
    {
        return Input.GetButton("Jump");
    }
    public override bool IsDashPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public override Vector2? GetAttackDirection()
    {
        return Input.GetMouseButtonDown(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }
    public override Vector2? GetContinuedAttackDirection()
    {
        return Input.GetMouseButton(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }
    public override bool IsReloadPressed()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
    public override bool IsSwapWeaponPressed()
    {
        return Input.GetKeyDown(KeyCode.Q); //TODO: confirm control with team
    }
    public override Vector2? GetHackInput()
    {
        return Input.GetMouseButtonDown(1) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }
}
