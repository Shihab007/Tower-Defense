using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitData data;

    private float attackTimer = 0f;
    private EnemyBase currentTarget;

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= 1f / data.attackSpeed)
        {
            attackTimer = 0f;
            FindTarget();
            if (currentTarget != null)
                Attack(currentTarget);
        }
    }

    void FindTarget()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (EnemyBase enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= data.range && dist < closestDistance)
            {
                closestDistance = dist;
                currentTarget = enemy;
            }
        }
    }

    void Attack(EnemyBase target)
    {
        Debug.Log($"{data.unitName} attacking {target.data.enemyName}");
        target.TakeDamage(data.damage);
    }
}