using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private List<SwitchController> _switchController;

    private bool _isOpened = false;

    // Update is called once per frame
    void Update()
    {
        CheckSwitches();
    }

    void CheckSwitches()
    {
        bool isOpenable = true;
        foreach (var item in _switchController)
        {
            if (!item.Clicked)
            {
                isOpenable = false;
                break;
            }
        }
        if (isOpenable)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }
    void OpenDoor()
    {
        _isOpened = true;
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
    void CloseDoor()
    {

        _isOpened = false;
        transform.localScale = new Vector3(0.2f, 2, 1);

    }
}
