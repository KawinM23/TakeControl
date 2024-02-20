using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransition : MonoBehaviour
{

    private Collider2D _collider;

    [SerializeField] private Object _toScene;
    private Scene _fromScene;


    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _fromScene = gameObject.scene;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _toScene.GetType().Equals(typeof(SceneAsset)))
        {
            Debug.Log("From " + _fromScene.name + " to " + _toScene.name);
            StartCoroutine(FindAnyObjectByType<MapManager>().ChangeScene(_fromScene.name, _toScene.name));
        }
    }

    public static Vector2 FindDestinationPosition(string fromScene, string toScene) {
        MapTransition[] mta = FindObjectsByType<MapTransition>(FindObjectsSortMode.None);
        foreach (MapTransition mt in mta)
        {
            Debug.Log("Door " + mt.gameObject.name +" "+(mt._fromScene.name.Equals(toScene) && mt._toScene.name.Equals(fromScene)));
            if (mt._fromScene.name.Equals(toScene)&& mt._toScene.name.Equals(fromScene))
            {
                return mt.transform.GetChild(0).position;
            }
        }
        return Vector2.zero;
    }
  
}
