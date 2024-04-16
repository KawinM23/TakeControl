using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private List<SwitchController> _switchController;
    [SerializeField] private List<SwitchController> _switchOppositeController;

    private bool _isOpened = false;
    private Vector3 position;

    private void Awake()
    {
        position = transform.localScale;
    }

    // Update is called once per frame

    void Update()
    {
        
        CheckSwitches();
       
    }

    void CheckSwitches()
    {
        bool _isOpenable = true; //all button in this list must active
        foreach (var item in _switchController)
        {
            if (!item.Clicked)
            {
                _isOpenable = false;
                break;
            }
        }

        // all button in this list must not active
        foreach (var item in _switchOppositeController)
        {
            if (item.Clicked)
            {
                _isOpenable = false;
                break;
            }
        }
        if (_isOpenable)
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
        transform.localScale = position;

    }
}
