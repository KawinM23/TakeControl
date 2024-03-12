using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public GameObject playerGameObject;
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
        if (!playerGameObject)
        {
            if (activePlayer)
            {
                playerGameObject = activePlayer;
            }
            else if (playerPrefab)
            {
                GameObject gameObject = Instantiate(playerPrefab);
                gameObject.SetActive(true);
                playerGameObject = gameObject;
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
            if (c.input is PlayerController && !ReferenceEquals(c.gameObject ,Instance.playerGameObject))
            {
                Destroy(c.gameObject);
            }
        }
    }

}
