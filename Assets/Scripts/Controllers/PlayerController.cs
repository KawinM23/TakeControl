using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    private bool isLeftMousePress = false;

    public override float RetrieveMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public override float RetrieveVerticalInput()
    {
        return Input.GetAxisRaw("Vertical");
    }
    public override bool RetrieveJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }
    public override bool RetrieveJumpHoldInput()
    {
        return Input.GetButton("Jump");
    }
    public override bool RetrieveDashInput()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public override bool RetrieveAttackInput()
    {
        return isLeftMousePress;
    }
    public override bool RetrieveReloadInput()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
    public override bool RetrieveSwapWeaponInput()
    {
        return Input.GetKeyDown(KeyCode.Q); //TODO: confirm control with team
    }
    public override Vector2? RetrieveHackInput()
    {
        return Input.GetMouseButtonDown(1) ? Camera.main.ScreenToWorldPoint(Input.mousePosition) : null;
    }

    public override void UpdateInputEventLoop()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isLeftMousePress = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isLeftMousePress = false;
        }
    }
}
