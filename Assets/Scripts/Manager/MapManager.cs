using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapTransition;

public class MapManager : MonoBehaviour
{
    private GameObject player;

    public static MapManager Instance { get; private set; }

    [SerializeField] private float mapChangeCooldown;
    public bool changingScene;

    public string fromScene;
    public string toScene;
    public float distanceFromSpawn;
    public Direction direction;

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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (changingScene)
        {
            player = PlayerManager.Instance.Player;
            player.transform.position = FindDestinationPosition(fromScene, toScene, distanceFromSpawn);
            player.SetActive(true);
            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                Debug.Log(PlayerPrefs.GetFloat("velocityX") + " " + PlayerPrefs.GetFloat("velocityY"));
                rb.velocity = new Vector2(PlayerPrefs.GetFloat("velocityX"), PlayerPrefs.GetFloat("velocityY"));
                if (direction == Direction.Up)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 2f);
                }
            }


            if (player.TryGetComponent(out TrailRenderer tr))
            {
                tr.Clear();
            }
            PlayerManager.DestroyOtherActivePlayers();
            changingScene = false;
        }
    }

    public IEnumerator ChangeScene(string fromSceneName, string toSceneName, Direction direction, float distanceFromSpawn)
    {
        if (changingScene)
        {
            yield break;
        }
        changingScene = true;
        fromScene = fromSceneName;
        toScene = toSceneName;
        this.distanceFromSpawn = distanceFromSpawn;
        this.direction = direction;

        player = PlayerManager.Instance.Player;
        if (player.TryGetComponent(out Rigidbody2D rb))
        {
            PlayerPrefs.SetFloat("velocityX", rb.velocity.x);
            PlayerPrefs.SetFloat("velocityY", rb.velocity.y);
        }

        DontDestroyOnLoad(player);
        player.SetActive(false);
        SceneManager.LoadScene(toSceneName);

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
