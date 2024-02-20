using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract float RetrieveMoveInput();
    public abstract bool RetrieveJumpInput();
    public abstract bool RetrieveJumpHoldInput();
    public abstract bool RetrieveAttackInput();
    public abstract bool RetrieveDashInput();
    public abstract bool RetrieveSwapWeaponInput();
    public abstract Vector2? RetrieveHackInput();
}
