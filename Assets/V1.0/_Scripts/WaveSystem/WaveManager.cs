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
    public int baseEnemyCount = 6;
    public float spawnInterval = 0.85f;
    public float timeBetweenWaves = 1.5f;

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

        int enemyCount = baseEnemyCount + (currentWave - 1) * 4;
        float speedMultiplier = 1f + (currentWave - 1) * 0.15f;
        float healthMultiplier = 1f + (currentWave - 1) * 0.25f;

        StartCoroutine(SpawnWave(enemyCount, speedMultiplier, healthMultiplier));
    }

    IEnumerator SpawnWave(int count, float speedMultiplier, float healthMultiplier)
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

            SpawnEnemy(speedMultiplier, healthMultiplier);
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;

        if (enemiesAlive <= 0)
            StartCoroutine(NextWaveDelay());
    }

    void SpawnEnemy(float speedMultiplier, float healthMultiplier)
    {
        GameObject obj = Instantiate(enemyPrefab);
        EnemyBase enemy = obj.GetComponent<EnemyBase>();

        if (enemy == null)
        {
            Debug.LogError("WaveManager: Enemy prefab does not have EnemyBase.");
            Destroy(obj);
            return;
        }

        EnemyData randomEnemyData = GetEnemyForWave();
        EnemyData scaledData = Instantiate(randomEnemyData);

        scaledData.moveSpeed *= speedMultiplier;
        scaledData.maxHealth *= healthMultiplier;

        enemy.Init(scaledData, waypointPath.waypoints);
    }

    EnemyData GetEnemyForWave()
    {
        List<EnemyData> availablePool = new List<EnemyData>();

        foreach (EnemyData enemy in enemyTypes)
        {
            if (enemy == null) continue;

            string nameLower = enemy.enemyName.ToLower();

            if (currentWave <= 2)
            {
                if (nameLower.Contains("runner") || nameLower.Contains("crawler"))
                    availablePool.Add(enemy);
            }
            else if (currentWave <= 4)
            {
                if (nameLower.Contains("runner") || nameLower.Contains("crawler") || nameLower.Contains("brute"))
                    availablePool.Add(enemy);
            }
            else if (currentWave <= 6)
            {
                if (nameLower.Contains("runner") || nameLower.Contains("crawler") || nameLower.Contains("brute") || nameLower.Contains("shield"))
                    availablePool.Add(enemy);
            }
            else
            {
                availablePool.Add(enemy);
            }
        }

        if (availablePool.Count == 0)
            availablePool = enemyTypes;

        return availablePool[Random.Range(0, availablePool.Count)];
    }

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
        yield return new WaitForSeconds(timeBetweenWaves);

        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            yield break;

        StartNextWave();
    }
}