using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    public static GameObject FindActivePlayer()
    {
        Controller[] controllers = FindObjectsByType<Controller>(FindObjectsSortMode.None);
        foreach (Controller c in controllers)
        {
            if (c.input is PlayerController)
            {
                return c.gameObject;
            }
        }
        return null;
    }
}
