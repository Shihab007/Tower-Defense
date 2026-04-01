using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public EnemyData data;

    private float currentHealth;
    private int waypointIndex = 0;
    private Transform[] waypoints;

    public void Init(EnemyData enemyData, Transform[] path)
    {
        data = enemyData;
        currentHealth = data.maxHealth;
        waypoints = path;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        if (waypointIndex >= waypoints.Length)
        {
            ReachedBase();
            return;
        }

        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * data.moveSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            waypointIndex++;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void ReachedBase()
    {
        GameManager.Instance.OnEnemyReachedBase();
        WaveManager.Instance.OnEnemyRemoved();
        Destroy(gameObject);
    }

    void Die()
    {
        WaveManager.Instance.OnEnemyRemoved();
        Destroy(gameObject);
    }
}