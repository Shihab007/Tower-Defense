using UnityEngine;

public class BulletBase : MonoBehaviour
{
    private EnemyBase target;
    private float damage;
    private float speed = 8f;
    private float hitDistance = 0.3f;
    private bool appliesSlow;
    private float slowMultiplier;
    private float slowDuration;
    private bool appliesChill;
    private int chillPerHit;
    private int freezeThreshold;
    private float freezeDuration;

    public void Init(
    EnemyBase enemy,
    float bulletDamage,
    bool shouldSlow = false,
    float slowMul = 1f,
    float slowDur = 0f,
    bool shouldApplyChill = false,
    int chillAmount = 0,
    int freezeAt = 0,
    float freezeDur = 0f)
    {
        target = enemy;
        damage = bulletDamage;

        appliesSlow = shouldSlow;
        slowMultiplier = slowMul;
        slowDuration = slowDur;

        appliesChill = shouldApplyChill;
        chillPerHit = chillAmount;
        freezeThreshold = freezeAt;
        freezeDuration = freezeDur;
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
            if (appliesSlow)
            {
                target.ApplySlow(slowMultiplier, slowDuration);
            }
            if (appliesChill)
            {
                target.ApplyChill(chillPerHit, freezeThreshold, freezeDuration);
            }
            Destroy(gameObject);
        }
    }
}