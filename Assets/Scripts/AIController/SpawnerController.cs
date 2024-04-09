using System.Collections;
using UnityEngine;

// SpawnerController spawns one type of GameObject in waves
public class SpawnerController : MonoBehaviour
{
    [SerializeField] private float _delayedStart = 0f; // The delay before the first wave
    [SerializeField] private GameObject _prefab; // The prefab to spawn
    [SerializeField] private int _spawnCountMax = 20;
    [SerializeField] private int _spawnCountMin = 1;
    [SerializeField] private float _spawnIntervalMax = 1f;
    [SerializeField] private float _spawnIntervalMin = 0f;

    [SerializeField] private int _waveCount = 12; // The number of waves to spawn
    [SerializeField] private float _waveInterval = 5f; // The interval between waves

    [SerializeField] private int _currentWave = 0; // The current wave number

    private void Start()
    {
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
            Instantiate(_prefab, spawnPosition, spawnRotation);

            // Randomize spawn interval
            float spawnInterval = Random.Range(_spawnIntervalMin, _spawnIntervalMax);

            // Wait for the spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }

        // Increase the wave number
        _currentWave++;
    }
}