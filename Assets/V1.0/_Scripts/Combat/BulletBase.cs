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
    private bool usesSplashDamage;
    private float splashRadius;
    private float splashDamageMultiplier;
    private bool usesChainDamage;
    private int chainCount;
    private float chainRadius;
    private float chainDamageMultiplier;

    private bool appliesStun;
    private float stunChance;
    private float stunDuration;

    public void Init(
    EnemyBase enemy,
    float bulletDamage,
    bool shouldSlow = false,
    float slowMul = 1f,
    float slowDur = 0f,
    bool shouldApplyChill = false,
    int chillAmount = 0,
    int freezeAt = 0,
    float freezeDur = 0f,
    bool shouldSplash = false,
    float aoeRadius = 0f,
    float aoeDamageMultiplier = 1f,
    bool shouldChain = false,
    int extraChainCount = 0,
    float extraChainRadius = 0f,
    float extraChainDamageMultiplier = 1f,
    bool shouldStun = false,
    float chanceToStun = 0f,
    float stunDur = 0f)
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

        usesSplashDamage = shouldSplash;
        splashRadius = aoeRadius;
        splashDamageMultiplier = aoeDamageMultiplier;

        usesChainDamage = shouldChain;
        chainCount = extraChainCount;
        chainRadius = extraChainRadius;
        chainDamageMultiplier = extraChainDamageMultiplier;

        appliesStun = shouldStun;
        stunChance = chanceToStun;
        stunDuration = stunDur;
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

            if (usesSplashDamage && splashRadius > 0f)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(target.transform.position, splashRadius);

                foreach (Collider2D hit in hits)
                {
                    EnemyBase nearbyEnemy = hit.GetComponent<EnemyBase>();

                    if (nearbyEnemy != null && nearbyEnemy != target)
                    {
                        nearbyEnemy.TakeDamage(damage * splashDamageMultiplier);
                    }
                }
            }

            if (usesChainDamage && chainCount > 0 && chainRadius > 0f)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(target.transform.position, chainRadius);

                int chained = 0;

                foreach (Collider2D hit in hits)
                {
                    EnemyBase nearbyEnemy = hit.GetComponent<EnemyBase>();

                    if (nearbyEnemy != null && nearbyEnemy != target)
                    {
                        nearbyEnemy.TakeDamage(damage * chainDamageMultiplier);
                        chained++;

                        if (chained >= chainCount)
                            break;
                    }
                }
            }
            
            if (appliesStun && stunChance > 0f && stunDuration > 0f)
            {
                if (Random.value <= stunChance)
                {
                    target.ApplyStun(stunDuration);
                }
            }

            Destroy(gameObject);
        }
    }
}