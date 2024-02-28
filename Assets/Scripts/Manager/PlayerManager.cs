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
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (!playerGameObject)
        {
            GameObject gameObject = Instantiate(playerPrefab);
            gameObject.SetActive(true);
            playerGameObject = gameObject;
        }
    }



}
