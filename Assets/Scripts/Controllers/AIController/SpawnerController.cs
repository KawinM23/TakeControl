using System.Collections;
using UnityEngine;

// SpawnerController spawns one type of GameObject in waves
public class SpawnerController : MonoBehaviour
{

    // prefabs
    [SerializeField] private GameObject _spawningGameObject; // The prefab to spawn
    [SerializeField] private GameObject _bomberPrefab;
    [SerializeField] private GameObject _swordChargerPrefab;
    [SerializeField] private GameObject _swordWielderPrefab;

    // spawner settings
    [SerializeField] private float _delayedStart = 0f; // The delay before the first wave
    [SerializeField] private int _spawnCountMax = 20;
    [SerializeField] private int _spawnCountMin = 1;
    [SerializeField] private float _spawnIntervalMax = 1f;
    [SerializeField] private float _spawnIntervalMin = 0f;

    [SerializeField] private int _waveCount = 12; // The number of waves to spawn
    [SerializeField] private float _waveInterval = 5f; // The interval between waves

    [SerializeField] private int _currentWave = 0; // The current wave number

    public enum SpawningGameObject
    {
        SWORD_CHARGER,
        BOMBER,
        SWORD_WIELDER
    }

    private void Start()
    {
        // If spawningGameObject is not set, use the default one
        if (_spawningGameObject == null)
        {
            _spawningGameObject = _swordChargerPrefab;
        }

        // Start spawning waves
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        if (_currentWave == 0)
        {
            yield return new WaitForSeconds(_delayedStart);
        }

        while (_currentWave < _waveCount)
        {
            // Spawn a wave
            yield return SpawnWave();

            // Wait for the wave interval
            yield return new WaitForSeconds(_waveInterval);
        }
    }

    private IEnumerator SpawnWave()
    {
        // Spawn enemies or objects in the wave
        // You can customize the spawn positions, rotations, and other configurations here

        // Randomize spawn count in range
        int spawnCount = Random.Range(_spawnCountMin, _spawnCountMax);

        // Example: Spawn 5 enemies in a line
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 0.5f, 0f);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(_spawningGameObject, spawnPosition, spawnRotation);

            // Randomize spawn interval
            float spawnInterval = Random.Range(_spawnIntervalMin, _spawnIntervalMax);

            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }

        // Increase the wave number
        _currentWave++;
    }

    // a class specifically created for SetSpawnSettings function parameters
    public class SpawnerSettings
    {
        public int? spawnCountMax;
        public int? spawnCountMin;
        public float? spawnIntervalMax;
        public float? spawnIntervalMin;
        public int? waveCount;
        public float? waveInterval;
        public int? currentWave;
        public float? delayedStart;
        public SpawningGameObject spawningGameObject;
    }
    public void SetSpawnerSettings(SpawnerSettings settings)
    {
        _spawnCountMax = settings.spawnCountMax != null ? settings.spawnCountMax.Value : _spawnCountMax;
        _spawnCountMin = settings.spawnCountMin != null ? settings.spawnCountMin.Value : _spawnCountMin;
        _spawnIntervalMax = settings.spawnIntervalMax != null ? settings.spawnIntervalMax.Value : _spawnIntervalMax;
        _spawnIntervalMin = settings.spawnIntervalMin != null ? settings.spawnIntervalMin.Value : _spawnIntervalMin;
        _waveCount = settings.waveCount != null ? settings.waveCount.Value : _waveCount;
        _waveInterval = settings.waveInterval != null ? settings.waveInterval.Value : _waveInterval;
        _currentWave = settings.currentWave != null ? settings.currentWave.Value : _currentWave;
        _delayedStart = settings.delayedStart != null ? settings.delayedStart.Value : _delayedStart;

        switch (settings.spawningGameObject)
        {
            case SpawningGameObject.SWORD_CHARGER:
                _spawningGameObject = _swordChargerPrefab;
                break;
            case SpawningGameObject.BOMBER:
                _spawningGameObject = _bomberPrefab;
                break;
            case SpawningGameObject.SWORD_WIELDER:
                _spawningGameObject = _swordWielderPrefab;
                break;
        }

    }
}