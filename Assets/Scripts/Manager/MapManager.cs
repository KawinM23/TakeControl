using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private GameObject player;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        player = Player.FindActivePlayer();
        Debug.Log(player);
        DontDestroyOnLoad(player);
    }

    public IEnumerator ChangeScene(string fromScene,string toScene)
    {
        yield return SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);
        SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(toScene));
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(toScene));
        player.transform.position = MapTransition.FindDestinationPosition(fromScene, toScene);
        player.GetComponent<TrailRenderer>().Clear();
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(fromScene));

        yield return null;
    }
}
