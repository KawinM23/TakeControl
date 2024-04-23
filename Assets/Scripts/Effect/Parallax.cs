using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Thank you @James for the base script
public class Parallax : MonoBehaviour
{

    [Header("Parallax Parameters")]
    public Vector2 factor;
    private Vector3 _startPos;

    // [Header("Scrolling")]
    // float scrollSpeedX = 0;

    private Vector3 _centerOfParallax;
    private Vector2 _deltaPos;

    void Start()
    {
        _startPos = transform.position;
        _centerOfParallax = Vector3.zero;
    }

    void Update()
    {
        var mainCamera = Camera.main;

        if (_centerOfParallax == null || mainCamera == null) return;

        _deltaPos = mainCamera.transform.position - _centerOfParallax;
        _deltaPos *= factor;
        this.transform.position = _startPos + (Vector3)_deltaPos;
    }
}
