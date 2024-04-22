using Assets.Scripts.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapUI : MonoBehaviour, IDataPersist
{
    [SerializeField] bool _visited = false;
    [SerializeField] bool _isInCurrentMap = false;

    [SerializeField] string _mapName;

    public void LoadData(in GameData data)
    {
        _visited = false;
        if (data.visited.TryGetValue(_mapName, out bool val))
        {
            _visited = val;
        }
        gameObject.SetActive(!_visited);
    }

    public void SaveData(ref GameData data)
    {
        data.visited[_mapName] = _visited;
    }

    private void Awake()
    {
        gameObject.SetActive(!_visited);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isInCurrentMap)
        {
            if (_mapName == SceneManager.GetActiveScene().name)
            {
                _isInCurrentMap = true;
                _visited = true;
                gameObject.SetActive(false);
                //Move pin
                Transform playerPin = transform.parent.Find("PlayerPin");
                if (playerPin) playerPin.position = transform.position;
            }
        }
        else
        {
            if (_mapName != SceneManager.GetActiveScene().name)
            {
                _isInCurrentMap = false;
            }
        }

    }
}
