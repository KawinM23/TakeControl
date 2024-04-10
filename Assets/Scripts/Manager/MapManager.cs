using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapTransition;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public bool IsChangingScene;
    public string FromScene;
    public string ToScene;
    public float DistanceFromSpawn;
    public Direction Direction;
    private GameObject _player;

    [SerializeField] private float _mapChangeCooldown;
    private float _mapChangeCooldownTimer;
    public bool CanChangeScene => _mapChangeCooldownTimer <= 0 && SceneManager.loadedSceneCount <= 1;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (_mapChangeCooldownTimer > 0)
        {
            _mapChangeCooldownTimer -= Time.deltaTime;
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerManager.SetRespawnScene(scene.name);
        if (IsChangingScene)
        {
            _player = PlayerManager.Instance.Player;
            _player.transform.position = FindDestinationPosition(FromScene, ToScene, DistanceFromSpawn);
            _player.SetActive(true);
            if (_player.TryGetComponent(out Rigidbody2D rb))
            {
                /*Debug.Log(PlayerPrefs.GetFloat("velocityX") + " " + PlayerPrefs.GetFloat("velocityY"));*/
                rb.velocity = new Vector2(PlayerPrefs.GetFloat("velocityX"), PlayerPrefs.GetFloat("velocityY"));
                if (Direction == Direction.Up)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 2f);
                }
            }

            if (_player.TryGetComponent(out TrailRenderer tr))
            {
                tr.Clear();
            }
            PlayerManager.DestroyOtherActivePlayers();
            IsChangingScene = false;
        }
    }

    public IEnumerator ChangeScene(string fromSceneName, string toSceneName, Direction direction, float distanceFromSpawn)
    {
        if (IsChangingScene && _mapChangeCooldownTimer > 0)
        {
            yield break;
        }
        IsChangingScene = true;
        FromScene = fromSceneName;
        ToScene = toSceneName;
        DistanceFromSpawn = distanceFromSpawn;
        Direction = direction;
        _mapChangeCooldownTimer = _mapChangeCooldown;

        _player = PlayerManager.Instance.Player;
        if (_player.TryGetComponent(out Rigidbody2D rb))
        {
            PlayerPrefs.SetFloat("velocityX", rb.velocity.x);
            PlayerPrefs.SetFloat("velocityY", rb.velocity.y);
        }

        DontDestroyOnLoad(_player);
        _player.SetActive(false);

        SaveManager.Instance.SaveData();
        
        yield return SceneManager.LoadSceneAsync(toSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(toSceneName));
        SceneManager.UnloadSceneAsync(fromSceneName);

        /*SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(toSceneName));*//*
        player.SetActive(true);
        player.transform.position = MapTransition.FindDestinationPosition(fromSceneName, toSceneName);
        *//*SceneManager.SetActiveScene(SceneManager.GetSceneByName(toSceneName));*//*

        if (player.TryGetComponent(out TrailRenderer tr))
        {
            tr.Clear();
        }


        *//*SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(fromSceneName));*//*
        changingScene = false;
        yield return null;*/
    }

}
