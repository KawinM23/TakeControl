using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private float mapChangeCooldown;
    public static bool changingScene;

    public static string fromScene;
    public static string toScene;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (changingScene)
        {
            player = Player.FindActivePlayer();
            player.SetActive(true);
            Debug.Log(fromScene + toScene);
            player.transform.position = MapTransition.FindDestinationPosition(fromScene, toScene);

            if (player.TryGetComponent(out TrailRenderer tr))
            {
                tr.Clear();
            }
            changingScene = false;
        }
    }

    public IEnumerator ChangeScene(string fromSceneName, string toSceneName)
    {
        if (changingScene)
        {
            yield break;
        }
        changingScene = true;
        fromScene = fromSceneName;
        toScene = toSceneName;

        player = Player.FindActivePlayer();
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
