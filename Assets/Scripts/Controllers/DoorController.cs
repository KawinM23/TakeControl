using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField]  List<SwitchController> _switchController;

    public bool _isDoorOpen = false;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool abletoopen = true;
        foreach (var item in _switchController)
        {
            if (!item._clicked)
            {
                abletoopen = false;
            }
        }
        if (abletoopen)
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
        _isDoorOpen = true;
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(0,0,0);
        }
    }
    void CloseDoor()
    {
       
        _isDoorOpen = false;
            transform.localScale = new Vector3(0.2f, 2,1);
        
    }
}
