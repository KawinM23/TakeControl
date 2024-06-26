using Assets.Scripts.SaveLoad;
using AYellowpaper.SerializedCollections;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapTransition;

public class MapManager : MonoBehaviour, IDataPersist
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

    [Header("Map UI")]
    [SerializeField] private SerializedDictionary<string, bool> _visited;
    [SerializeField] private GameObject _mapUI;

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

    private void Start()
    {
        MoveToMap(SceneManager.GetActiveScene().name);
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

            if (_player.TryGetComponent(out TrailRenderer tr))
            {
                tr.Clear();
            }
            PlayerManager.DestroyOtherActivePlayers();
        }
    }

    public void ChangeScene(string fromSceneName, string toSceneName, Direction direction, float distanceFromSpawn)
    {
        if (IsChangingScene && _mapChangeCooldownTimer > 0)
        {
            return;
        }
        IsChangingScene = true;
        FromScene = fromSceneName;
        ToScene = toSceneName;
        MoveToMap(toSceneName);
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

        StartCoroutine(LoadScene());

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

    private IEnumerator LoadScene()
    {
        yield return new WaitForEndOfFrame();
        yield return SceneManager.LoadSceneAsync(ToScene, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(FromScene);
        yield return StartCoroutine(OnSceneLoadCoroutine());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(ToScene));
        IsChangingScene = false;
    }

    private IEnumerator OnSceneLoadCoroutine()
    {
        if (PlayerManager.Instance.Player.TryGetComponent(out Rigidbody2D rb))
        {
            /*rb.AddForce(new Vector2(0, Direction == Direction.Up ? 100f : 0f));*/
            rb.velocity = new Vector2(_oldVelocity.x, (Direction == Direction.Up ? 8f : _oldVelocity.y));
        }
        yield return null;
    }

    public float GetMapChangeCooldown() { return _mapChangeCooldown; }

    public void MoveToMap(string toSceneName)
    {
        _visited[toSceneName] = true;
        Transform toSceneMapUI = _mapUI.transform.Find(toSceneName);
        if (toSceneMapUI == null) { return; }
        toSceneMapUI.gameObject.SetActive(false);
        Transform playerPin = _mapUI.transform.Find("PlayerPin");
        if (playerPin)
        {
            playerPin.localPosition = toSceneMapUI.localPosition;
        }
    }

    public void LoadData(in GameData data)
    {
        _visited = data.visited;
        foreach (string key in _visited.Keys)
        {
            if (_visited[key]) { _mapUI.transform.Find(key)?.gameObject.SetActive(false); }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.visited = _visited;
    }
}
