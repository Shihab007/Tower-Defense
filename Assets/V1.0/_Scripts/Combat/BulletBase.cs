using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private EnemyBase target;
    private float damage;
    private float speed = 8f;
    private float hitDistance = 0.3f;

    public void Init(EnemyBase enemyTarget, float bulletDamage)
    {
        target = enemyTarget;
        damage = bulletDamage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        float dist = Vector3.Distance(transform.position, target.transform.position);
        if (dist <= hitDistance)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}