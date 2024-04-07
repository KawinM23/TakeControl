using UnityEngine;
using System.Collections;

public abstract class BaseWeapon : MonoBehaviour
{
    // Use this for initialization
    protected abstract void Awake();

    // Update is called once per frame
    protected abstract void Update();
}
