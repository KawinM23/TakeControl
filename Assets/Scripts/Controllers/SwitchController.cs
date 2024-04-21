using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.SaveLoad;
using System;

public class SwitchController : MonoBehaviour, IDataPersist
{
    public bool Clicked = false;
    [SerializeField] private bool _isActivable;
    [SerializeField] private bool _isDeactivatable;

    private bool _isBeingPressed = false;
    private float _switchSizeY;
    private Vector3 _switchUpPos;
    private Vector3 _switchDownPos;
    private readonly float _switchSpeed = 1f;
    private readonly float _switchDelay = 1f;

    [SerializeField] string id;
    void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        _switchSizeY = transform.localScale.y / 2;
        _switchUpPos = transform.position;
        _switchDownPos = new Vector3(transform.position.x, transform.position.y - _switchSizeY, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

        if (Clicked)
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
        if (transform.position != _switchDownPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, _switchDownPos, _switchSpeed * Time.deltaTime);
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
            _isBeingPressed = !_isBeingPressed;
            if (_isActivable && !Clicked)
            {
                Clicked = true;
            }
            else if (_isDeactivatable && Clicked)
            {
                Clicked = false;
                SoundManager.Instance.PlayPressurePlateUp();
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
        yield return new WaitForSeconds(waitTime);
        _isBeingPressed = false;
    }

    public void SaveData(ref GameData data)
    {
        data.switches[id] = Clicked;
    }

    public void LoadData(in GameData data)
    {
        if (data.switches.TryGetValue(id, out bool val))
        {
            Clicked = val;
        }
    }

    [ContextMenu("Reroll All Id")]
    private void RerollAllId()
    {
        foreach (var x in FindObjectsOfType<SwitchController>())
        {
            x.id = Guid.NewGuid().ToString();
        }
    }
}
