using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransition : MonoBehaviour
{
    private Collider2D _collider;

    [SerializeField] private string _toScene;
    private Scene _fromScene;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _fromScene = gameObject.scene;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.TryGetComponent(out Controller controller);
        if (controller && controller.input is PlayerController)
        {
            Debug.Log("From " + _fromScene.name + " to " + _toScene);
            StartCoroutine(FindAnyObjectByType<MapManager>().ChangeScene(_fromScene.name, _toScene));
        }
    }

    public static Vector2 FindDestinationPosition(string fromScene, string toScene)
    {
        MapTransition[] mta = FindObjectsByType<MapTransition>(FindObjectsSortMode.None);
        foreach (MapTransition mt in mta)
        {
            if (mt._fromScene.name.Equals(toScene) && mt._toScene.Equals(fromScene))
            {
                return mt.transform.GetChild(0).position;
            }
        }
        return Vector2.zero;
    }
}
