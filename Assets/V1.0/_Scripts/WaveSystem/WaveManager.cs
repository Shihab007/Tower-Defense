using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public EnemyData enemyData;
    public GameObject enemyPrefab;
    public Transform[] waypoints;

    public int baseEnemyCount = 3;
    public float spawnInterval = 1.5f;
    public float timeBetweenWaves = 3f;

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool isSpawning = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartNextWave();
    }

    void StartNextWave()
    {
        if (GameManager.Instance.isGameOver) return;
    currentWave++;
    UIManager.Instance.UpdateWave(currentWave);
        currentWave++;
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
            SpawnEnemy(speedMultiplier);
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    void SpawnEnemy(float speedMultiplier)
    {
        GameObject obj = Instantiate(enemyPrefab, waypoints[0].position, Quaternion.identity);
        EnemyBase enemy = obj.GetComponent<EnemyBase>();

        EnemyData scaledData = Instantiate(enemyData);
        scaledData.moveSpeed *= speedMultiplier;
        scaledData.maxHealth *= 1f + (currentWave - 1) * 0.15f;

        enemy.Init(scaledData, waypoints);
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
    if (GameManager.Instance.isGameOver) yield break;
    StartNextWave();
    }
}