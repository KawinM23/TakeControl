using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : ScriptableObject, InputController
{
    public float GetHorizontalMovement()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public float GetVerticalMovement()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public bool IsJumpPressed()
    {
        return Input.GetButtonDown("Jump");
    }
    public bool IsJumpHeld()
    {
        return Input.GetButton("Jump");
    }
    public bool IsDashPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public Vector2? GetAttackDirection()
    {
        return Input.GetMouseButtonDown(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }
    public Vector2? GetContinuedAttackDirection()
    {
        return Input.GetMouseButton(0) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }
    public bool IsReloadPressed()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
    public bool IsSwapWeaponPressed()
    {
        return Input.GetKeyDown(KeyCode.Q); //TODO: confirm control with team
    }
    public Vector2? GetHackInput()
    {
        return Input.GetMouseButtonDown(1) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }
}
