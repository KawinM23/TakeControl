using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }
    public static event UnityAction<GameObject> OnPlayerChanged;

    public GameObject Player
    {
        get => _player;
        set
        {
            _player = value;
            OnPlayerChanged?.Invoke(_player);
        }
    }
    private GameObject _player;

    [SerializeField] private GameObject playerPrefab;

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
            else if (playerPrefab)
            {
                GameObject gameObject = Instantiate(playerPrefab);
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
            if (c.input is PlayerController && c.gameObject.activeSelf)
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
            if (c.input is PlayerController && !ReferenceEquals(c.gameObject, Instance.Player))
            {
                Destroy(c.gameObject);
            }
        }
    }

}
