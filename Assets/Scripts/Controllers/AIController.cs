using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIController", menuName = "InputController/AIController")]
public class AIController : InputController
{

    public override float RetrieveMoveInput()
    {
        return 1f;
    }
    public override bool RetrieveJumpInput()
    {
        return true;
    }
    public override bool RetrieveJumpHoldInput()
    {
        return false;
    }
    public override bool RetrieveAttackInput()
    {
        return true;
    }

    public override bool RetrieveDashInput()
    {
        return false;
    }
    public override bool RetrieveSwapWeaponInput()
    {
        throw new System.NotImplementedException();
    }
}
