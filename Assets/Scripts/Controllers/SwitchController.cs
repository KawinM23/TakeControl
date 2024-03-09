using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    [SerializeField] DoorController _doorController;
    [SerializeField] bool _isDoorOpenSwitch;
    [SerializeField] bool _isDoorCloseSwitch;


    float _switchSizeY;
    Vector3 _switchUpPos;
    Vector3 _switchDownPos;
    float _switchSpeed = 1f;
    float _switchDelay = 0.3f;
    bool _isPressingSwitch = false;
    // Start is called before the first frame update
    private void Awake()
    {
        _switchSizeY = transform.localScale.y/2;
        _switchUpPos = transform.position;
        _switchDownPos = new Vector3(transform.position.x,transform.position.y - _switchSizeY,transform.position.z);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (_isPressingSwitch)
        {
            MoveSwitchDown();
        }
        else
        {
            MoveSwitchUp();
        }

    }

    void MoveSwitchDown()
    {
        if(transform.position != _switchDownPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchDownPos, _switchSpeed*Time.deltaTime);
        }
    }

    void MoveSwitchUp()
    {
        if (transform.position != _switchUpPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchUpPos, _switchSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _isPressingSwitch = !_isPressingSwitch;
            if(_isDoorOpenSwitch && !_doorController._isDoorOpen)
            {
                _doorController._isDoorOpen = !_doorController._isDoorOpen;
            }
            else if(_isDoorCloseSwitch && _doorController._isDoorOpen)
            {
                Debug.Log("Closing");
                _doorController._isDoorOpen = !_doorController._isDoorOpen;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SwitchUpDelay(_switchDelay));
        }
    }
    IEnumerator SwitchUpDelay(float waitTime)
    {
        yield return new   WaitForSeconds(waitTime);
        _isPressingSwitch = false;
    }
}
