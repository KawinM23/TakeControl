using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIController", menuName = "InputController/AIController")]
public class AIController : InputController
{
    public override float RetrieveMoveInput()
    {
        return 0f;
    }
    public override float RetrieveVerticalInput()
    {
        return 0f;
    }
    public override bool RetrieveJumpInput()
    {
        return false;
    }
    public override bool RetrieveJumpHoldInput()
    {
        return false;
    }

    public override bool RetrieveAttackInput()
    {
        return false;
    }

    public override bool RetrieveDashInput()
    {
        return false;
    }
    public override bool RetrieveSwapWeaponInput()
    {
        return false;
    }

    public override Vector2? RetrieveHackInput()
    {
        return null;
    }
}
