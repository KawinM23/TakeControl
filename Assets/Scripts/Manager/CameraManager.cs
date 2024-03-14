using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Assets.Scripts.Capabilities;
using UnityEngine.SceneManagement;
using TMPro;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _camera;

    // Start is called before the first frame update
    void Start()
    {
        if (_camera == null)
        {
            _camera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        Debug.Assert(_camera != null, "Camera is not set in CameraManager");

        _camera.Follow = PlayerManager.Instance.playerGameObject.transform;
        PlayerManager.OnPlayerChanged += (player) => _camera.Follow = player.transform;


        // Remove non-cinemachine cameras
        // SceneManager.sceneLoaded += (_, _) =>
        // {
        //     foreach (var c in FindObjectsByType<Camera>(FindObjectsSortMode.None))
        //     {
        //         if (!c.TryGetComponent<CinemachineBrain>(out var _))
        //         {
        //             Debug.Log("Destroying camera");
        //             Destroy(c.gameObject);
        //         }
        //     }
        // };
    }
}
