using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private InterfaceReference<InputController> input = null;
    public InputController Input
    {
        get
        {
            return input.Value;
        }
        set
        {
            input.Value = value;
        }
    }
}
