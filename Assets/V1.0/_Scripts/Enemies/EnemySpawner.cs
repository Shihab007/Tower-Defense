using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public EnemyData enemyData;
    public Transform[] waypoints;

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        GameObject obj = Instantiate(enemyPrefab, waypoints[0].position, Quaternion.identity);
        EnemyBase enemy = obj.GetComponent<EnemyBase>();
        enemy.Init(enemyData, waypoints);
    }
}