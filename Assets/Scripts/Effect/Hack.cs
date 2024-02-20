using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Combat;

namespace Assets.Scripts.Effect
{
    public class Hack : MonoBehaviour
    {
        [SerializeField] private float _rotSpeed = 10.0f;

        private void Update()
        {
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * _rotSpeed));
        }
    }
}
