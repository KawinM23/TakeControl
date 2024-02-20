using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    private GameObject player;

    [SerializeField] private float mapChangeCooldown;
    public static bool changingScene;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator ChangeScene(string fromScene, string toScene)
    {
        if (changingScene)
        {
            yield break;
        }
        changingScene = true;

        player = Player.FindActivePlayer();
        DontDestroyOnLoad(player);

        yield return SceneManager.LoadSceneAsync(toScene, LoadSceneMode.Additive);

        SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneByName(toScene));
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(toScene));
        player.transform.position = MapTransition.FindDestinationPosition(fromScene, toScene);
        if (player.TryGetComponent(out TrailRenderer tr))
        {
            tr.Clear();
        }

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(fromScene));
        changingScene = false;
        yield return null;
    }
}
