using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    //Movement
    public abstract float RetrieveMoveInput();
    public abstract float RetrieveVerticalInput();
    public abstract bool RetrieveJumpInput();
    public abstract bool RetrieveJumpHoldInput();
    public abstract bool RetrieveDashInput();

    public abstract bool RetrieveAttackInput();
    public abstract bool RetrieveReloadInput();
    public abstract bool RetrieveSwapWeaponInput();

    public abstract Vector2? RetrieveHackInput();

    public abstract void UpdateInputEventLoop();
}
