using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Path")]
    public WaypointPath waypointPath;

    [Header("Enemy Setup")]
    public List<EnemyData> enemyTypes;
    public GameObject enemyPrefab;

    [Header("Wave Settings")]
    public int baseEnemyCount = 3;
    public float spawnInterval = 1.5f;
    public float timeBetweenWaves = 3f;

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (!ValidateSetup()) return;
        StartNextWave();
    }

    bool ValidateSetup()
    {
        if (waypointPath == null)
        {
            Debug.LogError("WaveManager: WaypointPath is not assigned.");
            return false;
        }

        if (waypointPath.waypoints == null || waypointPath.waypoints.Length == 0)
        {
            Debug.LogError("WaveManager: WaypointPath has no waypoints.");
            return false;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("WaveManager: Enemy prefab is not assigned.");
            return false;
        }

        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogError("WaveManager: No enemy types assigned.");
            return false;
        }

        return true;
    }

    void StartNextWave()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;
        if (isSpawning) return;

        currentWave++;
        UIManager.Instance?.UpdateWave(currentWave);

        int enemyCount = baseEnemyCount + (currentWave - 1) * 2;
        float speedMultiplier = 1f + (currentWave - 1) * 0.1f;

        Debug.Log($"Wave {currentWave} started. Enemies: {enemyCount}");
        StartCoroutine(SpawnWave(enemyCount, speedMultiplier));
    }

    IEnumerator SpawnWave(int count, float speedMultiplier)
    {
        isSpawning = true;
        enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                isSpawning = false;
                yield break;
            }

            SpawnEnemy(speedMultiplier);
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;

        if (enemiesAlive <= 0)
            StartCoroutine(NextWaveDelay());
    }

    void SpawnEnemy(float speedMultiplier)
    {
        GameObject obj = Instantiate(enemyPrefab);
        EnemyBase enemy = obj.GetComponent<EnemyBase>();

        if (enemy == null)
        {
            Debug.LogError("WaveManager: Enemy prefab does not have EnemyBase.");
            Destroy(obj);
            return;
        }

        EnemyData randomEnemyData = enemyTypes[Random.Range(0, enemyTypes.Count)];
        EnemyData scaledData = Instantiate(randomEnemyData);

        scaledData.moveSpeed *= speedMultiplier;
        scaledData.maxHealth *= 1f + (currentWave - 1) * 0.15f;

        enemy.Init(scaledData, waypointPath.waypoints);
    }

    // public void OnEnemyRemoved()
    // {
    //     enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

    //     if (enemiesAlive <= 0 && !isSpawning)
    //         StartCoroutine(NextWaveDelay());
    // }
    public void RegisterSpawnedEnemy()
    {
        enemiesAlive++;
    }

    public void OnEnemyRemoved()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && !isSpawning)
            StartCoroutine(NextWaveDelay());
    }

    IEnumerator NextWaveDelay()
    {
        Debug.Log($"Wave {currentWave} cleared. Next wave in {timeBetweenWaves}s");
        yield return new WaitForSeconds(timeBetweenWaves);

        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            yield break;

        StartNextWave();
    }
}