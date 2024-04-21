using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class TrackFollower : MonoBehaviour
{
    [Tooltip("The LineRenderer object that represents the track")]
    [SerializeField] private LineRenderer _track;

    [Tooltip("The speed of the object moving along the track")]
    [SerializeField] private float _speed = 2f;

    [Tooltip("The distance to the target position to consider as reached")]
    [SerializeField] private float _torelance = 0.1f;

    [Tooltip("At the start, should object instantly move to the first point")]
    [SerializeField] private bool _snapToStart = true;

    private int _curIdx = 0;
    private bool _incrementing = true;


    void Start()
    {
        if (_track == null)
        {
            Debug.LogError("No LineRenderer found or assign on object");
            return;
        }

        if (_snapToStart)
        {
            transform.position = _track.GetPosition(0);
        }
    }

    private void FixedUpdate()
    {
        var tarPos = _track.GetPosition(_curIdx);

        // Check if we reached the target position
        if (Vector3.Distance(transform.position, tarPos) < _torelance)
        {
            _curIdx += _incrementing ? 1 : -1;

            if (_curIdx >= _track.positionCount)
            {
                if (_track.loop)
                {
                    _curIdx = 0;
                }
                else
                {
                    _curIdx = _track.positionCount - 2;
                    _incrementing = false;
                }
            }

            if (_curIdx < 0)
            {
                if (_track.loop)
                {
                    _curIdx = _track.positionCount - 1;
                }
                else
                {
                    _curIdx = 1;
                    _incrementing = true;
                }
            }
        }

        // Move towards the target position
        var ds = _speed * Time.fixedDeltaTime * (tarPos - transform.position).normalized;
        transform.position += ds;
    }
}
