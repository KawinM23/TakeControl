using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool _isDoorOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDoorOpen)
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
        if(transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(0,0,0);
        }
    }
    void CloseDoor()
    {
       

            transform.localScale = new Vector3(0.2f, 2,1);
        
    }
}
