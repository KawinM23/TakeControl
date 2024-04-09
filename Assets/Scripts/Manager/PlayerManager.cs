using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For UI elements


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public static event UnityAction<GameObject> OnPlayerChanged;
    public static event UnityAction OnPlayerDied;

    public GameObject Player
    {
        get => _player;
        set
        {
            _player = value;
            if (_player != null)
                OnPlayerChanged?.Invoke(_player);
        }
    }
    private GameObject _player;
    [SerializeField] private bool _isDead = false; // Flag to track player death
    [SerializeField] private Canvas _respawnCanvas; // Reference to the respawn canvas
    private static string _respawnScene = ""; // starts as blank, change on first scene load and checkpoint trigger

    [SerializeField] private GameObject _playerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            SetUp();
        }
    }

    private void Start()
    {
        SetUp();
        _respawnCanvas.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            OnPlayerChanged = null;
            OnPlayerDied = null;
        }
    }

    public void Die()
    {
        Debug.Log("Player is dead");
        _respawnCanvas.gameObject.SetActive(true); // Show respawn canvas

        OnPlayerDied?.Invoke();
    }
    public void Respawn() // Called when respawn button is clicked
    {
        Debug.Log("Respawning...");
        _isDead = false; // Reset death flag
        Debug.Log("Reloading scene: " + _respawnScene);
        // SceneManager.LoadScene(_respawnScene); // Reload current scene
        _respawnCanvas.gameObject.SetActive(false); // Hide respawn canvas
        SaveManager.Instance.LoadSave();
    }

    private void SetUp()
    {
        GameObject activePlayer = FindActivePlayer();
        if (!Player)
        {
            if (activePlayer)
            {
                Player = activePlayer;
            }
            else if (_playerPrefab)
            {
                GameObject gameObject = Instantiate(_playerPrefab);
                gameObject.SetActive(true);
                Player = gameObject;
            }
        }
    }

    public static GameObject FindActivePlayer()
    {
        Controller[] controllers = FindObjectsByType<Controller>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Controller c in controllers)
        {
            if (c.Input is PlayerController && c.gameObject.activeSelf)
            {
                return c.gameObject;
            }
        }
        return null;
    }

    public static void DestroyOtherActivePlayers()
    {
        Controller[] controllers = FindObjectsByType<Controller>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Controller c in controllers)
        {
            if (c.Input is PlayerController && !ReferenceEquals(c.gameObject, Instance.Player))
            {
                Destroy(c.gameObject);
            }
        }
    }

    public static void SetRespawnScene(string sceneName)
    {
        if (_respawnScene == "")
        {
            Debug.Log("Setting respawn scene to: " + sceneName);
            _respawnScene = sceneName;
        }

    }

}
