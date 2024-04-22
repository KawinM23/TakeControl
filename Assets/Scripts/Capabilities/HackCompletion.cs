using UnityEngine;

public abstract class HackCompletion : MonoBehaviour
{

    public abstract void OnHackSuccess();
    public abstract void OnHackFail();
    public abstract void OnComboHackSuccess(int comboCount);
    public abstract void OnComboHackFail(int comboCount);
}