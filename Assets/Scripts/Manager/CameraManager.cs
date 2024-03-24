using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private CinemachineVirtualCamera _camera;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (_camera == null)
        {
            _camera = FindObjectOfType<CinemachineVirtualCamera>();
        }
        Debug.Assert(_camera != null, "Camera is not set in CameraManager");
        _camera.Follow = PlayerManager.Instance.Player.transform;

        PlayerManager.OnPlayerChanged += (player) => _camera.Follow = player.transform;

        SceneManager.sceneLoaded += (_, _) =>
        {
            _camera = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None).First(
                obj => obj != _camera
            );
            _camera.Follow = PlayerManager.Instance.Player.transform;

            // RemoveNonCinemachineCameras();
        };
    }

    private void RemoveNonCinemachineCameras()
    {
        foreach (var c in FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            if (!c.TryGetComponent<CinemachineBrain>(out var _))
            {
                Debug.Log("Destroying camera");
                Destroy(c.gameObject);
            }
        }
    }
}
