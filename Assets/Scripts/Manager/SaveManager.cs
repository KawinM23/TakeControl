using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.SaveLoad;
using UnityEngine;
using UnityEngine.SceneManagement;

#nullable enable annotations

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public event Action? InitialLoaded;

    [SerializeField] private GameData _gameData;
    [SerializeField] private string defaultSaveName = "default";
    [SerializeField] private string defaultSceneName;

    /// <summary>
    /// Action to run one time, once the SaveManage instance is created.
    /// 
    /// It's somewhat hacky way to continue running script from old instance across scene load, 
    /// which would destroy the old instance, but since this is static it will persist.
    /// 
    /// <seealso cref="AfterInitialLoad(SaveManager)"/>
    /// </summary> 
    private static Action<SaveManager>? _afterLoader;
    private List<IDataPersist> _dataPersists;
    private ISaver _saver;

#nullable enable

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("SaveLoadManager already exists in the scene. Deleting duplicate.");
            Destroy(this.gameObject);
            return;
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

        // Sanity check on default scene name
        if (string.IsNullOrEmpty(defaultSceneName))
        {
            Debug.LogError("Default scene name is not set.");
        }
        else if (SceneUtility.GetBuildIndexByScenePath(defaultSceneName) == -1)
        {
            Debug.LogError("Scene with name " + defaultSceneName + " not found.");
        }
    }

    void Start()
    {
        // Run after load if has been requested for previous instance
        if (_afterLoader != null)
        {
            _afterLoader(this);
            _afterLoader = null;
        }
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
        LoadData();
    }

    /// <summary>
    /// Save data from all IDataPersist objects into the GameData object
    ///  
    /// should be called before every scene transition.
    /// </summary>
    public void SaveData()
    {
        foreach (var dataPersist in _dataPersists)
        {
            dataPersist.SaveData(ref _gameData);
        }
    }

    /// <summary>
    /// Load data from the GameData object into all IDataPersist objects.
    ///  
    /// should be called after every scene transition.
    /// </summary>
    public void LoadData()
    {
        foreach (var dataPersist in _dataPersists)
        {
            dataPersist.LoadData(in _gameData);
        }
    }


    // TODO: handle multiple saves later (if needed)

    [ContextMenu("New Game")]
    public void NewGame()
    {
        Debug.Log("New Game");
        this._gameData = new GameData
        {
            name = defaultSaveName,
            currentScene = defaultSceneName
        };

        InitialLoad();
    }

    [ContextMenu("Save")]
    public void PersistSave()
    {
        if (string.IsNullOrEmpty(_gameData.name))
        {
            _gameData.name = defaultSaveName;
        }
        _gameData.currentScene = SceneManager.GetActiveScene().name;

        SaveData();
        _saver.PersistSave(_gameData, defaultSaveName);
    }

    [ContextMenu("Load")]
    public void LoadSave()
    {
        // TODO: don't use exceptions for control flow..., maybe use optional
        try
        {
            _gameData = _saver.LoadSave(defaultSaveName);
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarning("No save file found. Starting new game.");
            NewGame();
            return;
        }
        InitialLoad();
    }

    [ContextMenu("Delete")]
    public void DeleteSave()
    {
        _saver.DeleteSave(defaultSaveName);
    }

    public IEnumerable<string> ListSave()
    {
        return _saver.ListSaves();
    }

    /// <summary>
    /// Initialize the game state on first load (eg. on new game or load from save)
    /// </summary>
    private void InitialLoad()
    {
        // Prepare to remove old player and manager (except this one)
        var player = PlayerManager.Instance?.Player;
        if (player != null)
        {
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetActiveScene());
        }
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

        // Prepare after loader to run after scene load
        _afterLoader = (newThis) =>
        {
            this.AfterInitialLoad(newThis);
        };

        SceneManager.LoadScene(_gameData.currentScene, LoadSceneMode.Single);
        InitialLoaded?.Invoke();
    }

    /// <summary>
    /// Should run after the `InitialLoad` method finished loading the new scene.
    ///  
    /// Once the scene is loaded, new instance of SaveManager is created and this method is called
    /// with the new instance as argument.
    /// 
    /// See <see cref="_afterLoader"/> for mechanism which run this method.
    /// </summary>
    /// <param name="newThis">The new instance of SaveManager (on the new scene)</param>
    void AfterInitialLoad(SaveManager newThis)
    {
        // Load using the data from the previous instance
        newThis._gameData = this._gameData;
        newThis.LoadData();

        // Move player to save point in the scene (if exists)
        var sp = FindObjectOfType<SavePoint>();
        if (!sp)
        {
            Debug.LogWarning("No save point found in the scene.");
        }
        var player = PlayerManager.Instance != null ? PlayerManager.Instance.Player : null;
        if (sp && player != null)
        {
            player.transform.position = sp.transform.position;
        }
    }

    private List<IDataPersist> FindAllDataPersist()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersist>().ToList();
    }
}
