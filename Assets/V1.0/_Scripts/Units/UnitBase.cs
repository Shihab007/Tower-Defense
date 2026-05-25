using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitData data;
    public Vector2Int currentCell;
    public GameObject bulletPrefab;

    private float attackTimer = 0f;
    private float manaTickTimer = 0f;
    private EnemyBase currentTarget;
    private SpriteRenderer spriteRenderer;
    private Transform visualTransform;
    private SummonManager summonManager;

    void Awake()
    {
        visualTransform = transform.Find("Visual");

        if (visualTransform != null)
            spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
        else
            spriteRenderer = GetComponent<SpriteRenderer>();

        summonManager = FindFirstObjectByType<SummonManager>();
    }

    void Update()
    {
        if (data == null) return;

        HandleManaGeneration();

        attackTimer += Time.deltaTime;

        if (attackTimer >= 1f / data.attackSpeed)
        {
            attackTimer = 0f;
            FindTarget();

            if (currentTarget != null)
                Attack(currentTarget);
        }
    }

    public void Initialize(UnitData unitData, Vector2Int cell)
    {
        data = unitData;
        currentCell = cell;
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        if (data == null) return;

        transform.localScale = Vector3.one;

        if (visualTransform != null)
            visualTransform.localScale = data.visualScale;

        if (spriteRenderer != null)
        {
            if (data.unitSprite != null)
                spriteRenderer.sprite = data.unitSprite;

            spriteRenderer.color = data.unitColor;
        }
    }

    void HandleManaGeneration()
    {
        if (data == null || !data.generatesMana) return;
        if (summonManager == null) return;
        if (data.manaPerTick <= 0 || data.manaTickInterval <= 0f) return;

        manaTickTimer += Time.deltaTime;

        if (manaTickTimer >= data.manaTickInterval)
        {
            manaTickTimer = 0f;

            if (summonManager.currentMana < summonManager.maxMana)
            {
                summonManager.AddMana(data.manaPerTick);
            }
        }
    }

    void FindTarget()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        foreach (EnemyBase enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);

            // Global targeting: no range restriction
            if (dist < closestDistance)
            {
                closestDistance = dist;
                currentTarget = enemy;
            }
        }
    }

    void Attack(EnemyBase target)
    {
        if (bulletPrefab == null || target == null) return;

        GameObject obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        BulletBase bullet = obj.GetComponent<BulletBase>();
        bullet.Init(
            target,
            data.damage,
            data.appliesSlow,
            data.slowMultiplier,
            data.slowDuration,
            data.appliesChill,
            data.chillPerHit,
            data.freezeThreshold,
            data.freezeDuration,
            data.usesSplashDamage,
            data.splashRadius,
            data.splashDamageMultiplier,
            data.usesChainDamage,
            data.chainCount,
            data.chainRadius,
            data.chainDamageMultiplier,
            data.appliesStun,
            data.stunChance,
            data.stunDuration
        );
    }
}