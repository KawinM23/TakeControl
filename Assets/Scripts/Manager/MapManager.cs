using System.Collections;
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
    private Vector2 _oldVelocity;

    [SerializeField] private float _mapChangeCooldown;
    private float _mapChangeCooldownTimer;
    public bool CanChangeScene => _mapChangeCooldownTimer <= 0 && SceneManager.loadedSceneCount <= 1;

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
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
                rb.velocity = new Vector2(_oldVelocity.x, (Direction == Direction.Up ? 7f : _oldVelocity.y));
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
            _oldVelocity = rb.velocity;
        }

        DontDestroyOnLoad(_player);
        _player.SetActive(false);

        SaveManager.Instance.SaveData();

        yield return new WaitForEndOfFrame();
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
