using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private GameData _gameData;
    [SerializeField] private string defaultSaveName = "default";

    private List<IDataPersist> _dataPersists;
    private ISaver _saver;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("SaveLoadManager already exists in the scene. Deleting duplicate.");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        this._saver = new FileSaver(
            new JsonSerializer(),
            Application.persistentDataPath,
            ".json"
        );
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _dataPersists = FindAllDataPersist();
        LoadSave();
    }

    void OnChangingScene(string fromScene, string toScene)
    {
        Debug.Log("Changing scene from " + fromScene + " to " + toScene);
        // PersistSave();
    }

    // TODO: handle multiple saves later (if needed)


    public void NewGame()
    {
        Debug.Log("New Game");
        this._gameData = new GameData
        {
            name = defaultSaveName
        };

        // TODO: reload game somehow maybe
    }

    [ContextMenu("Save")]
    public void PersistSave()
    {
        if (string.IsNullOrEmpty(_gameData.name))
        {
            _gameData = new GameData
            {
                name = defaultSaveName
            };
        }
        _gameData.currentScene = SceneManager.GetActiveScene().name;

        foreach (var dataPersist in _dataPersists)
        {
            dataPersist.SaveData(ref _gameData);
        }

        _saver.PersistSave(_gameData, defaultSaveName);
    }

    [ContextMenu("Load")]
    private void LoadSave()
    {
        _gameData = _saver.LoadSave(defaultSaveName);

        // SceneManager.LoadScene(_gameData.currentScene);
        foreach (var dataPersist in _dataPersists)
        {
            dataPersist.LoadData(in _gameData);
        }
    }

    [ContextMenu("Delete")]
    private void DeleteSave()
    {
        _saver.DeleteSave(defaultSaveName);
    }

    private List<IDataPersist> FindAllDataPersist()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersist>().ToList();
    }
}
