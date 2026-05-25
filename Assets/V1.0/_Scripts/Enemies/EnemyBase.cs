using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public EnemyData data;

    private float currentHealth;
    private float currentShieldHealth;
    private int waypointIndex = 0;
    private Transform[] waypoints;
    private bool isInitialized = false;
    private bool isRemoved = false;

    private Transform visualTransform;
    private SpriteRenderer spriteRenderer;
    private Vector3 visualBaseLocalPosition;

    private float slowTimer = 0f;
    private float currentSlowMultiplier = 1f;
    private int currentChillStacks = 0;
    private float freezeTimer = 0f;
    private float stunTimer = 0f;

    void Awake()
    {
        visualTransform = transform.Find("Visual");

        if (visualTransform != null)
        {
            spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
            visualBaseLocalPosition = visualTransform.localPosition;
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (multiplier <= 0f) return;

        currentSlowMultiplier = Mathf.Min(currentSlowMultiplier, multiplier);
        slowTimer = Mathf.Max(slowTimer, duration);
    }

    public void ApplyChill(int chillAmount, int threshold, float freezeDuration)
    {
        if (chillAmount <= 0 || threshold <= 0 || freezeDuration <= 0f) return;

        currentChillStacks += chillAmount;

        if (currentChillStacks >= threshold)
        {
            freezeTimer = freezeDuration;
            currentChillStacks = 0;
        }
    }

    public void ApplyStun(float duration)
    {
        if (duration <= 0f) return;

        stunTimer = Mathf.Max(stunTimer, duration);
    }
    public void InitAtPoint(EnemyData enemyData, Transform[] pathWaypoints, Vector3 spawnPosition, int startWaypointIndex)
    {
        data = enemyData;
        waypoints = pathWaypoints;

        if (data == null)
        {
            Debug.LogError("EnemyBase: EnemyData is null.");
            return;
        }

        currentHealth = data.maxHealth;
        currentShieldHealth = data.hasShield ? data.shieldHealth : 0f;

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("EnemyBase: No waypoints assigned.");
            return;
        }

        ApplyVisuals();

        transform.position = spawnPosition;
        waypointIndex = Mathf.Clamp(startWaypointIndex, 0, waypoints.Length - 1);
        isInitialized = true;
    }

    public void Init(EnemyData enemyData, Transform[] pathWaypoints)
    {
        data = enemyData;
        waypoints = pathWaypoints;

        if (data == null)
        {
            Debug.LogError("EnemyBase: EnemyData is null.");
            return;
        }

        currentHealth = data.maxHealth;
        currentShieldHealth = data.hasShield ? data.shieldHealth : 0f;

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("EnemyBase: No waypoints assigned.");
            return;
        }

        ApplyVisuals();

        // transform.position = waypoints[0].position;
        // waypointIndex = 1;
        // isInitialized = true;

        int startIndex = 2;

        if (waypoints.Length <= startIndex)
        {
            Debug.LogError("EnemyBase: Not enough waypoints for requested start index.");
            return;
        }

        transform.position = waypoints[startIndex].position;
        waypointIndex = startIndex + 1;
        isInitialized = true;
    }

    void ApplyVisuals()
    {
        transform.localScale = Vector3.one;

        if (visualTransform != null)
        {
            visualTransform.localScale = data.visualScale;
            visualTransform.localPosition = visualBaseLocalPosition;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = data.enemySprite;
            spriteRenderer.color = data.enemyColor;
            spriteRenderer.enabled = data.enemySprite != null;
        }
    }

    void Update()
    {
        if (!isInitialized) return;
        if (GameManager.Instance != null && GameManager.Instance.isGameOver) return;

        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0f)
            {
                slowTimer = 0f;
                currentSlowMultiplier = 1f;
            }
        }

        if (freezeTimer > 0f)
        {
            freezeTimer -= Time.deltaTime;

            if (freezeTimer < 0f)
                freezeTimer = 0f;
        }

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer < 0f)
                stunTimer = 0f;
        }

        Move();
        AnimateVisual();
    }

    void Move()
    {
        if (freezeTimer > 0f || stunTimer > 0f)
        {
            return;
        }
        if (waypoints == null || waypoints.Length == 0) return;

        if (waypointIndex >= waypoints.Length)
        {
            ReachedBase();
            return;
        }

        Transform target = waypoints[waypointIndex];
        Vector3 previousPosition = transform.position;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            (data.moveSpeed * currentSlowMultiplier) * Time.deltaTime
        );

        Vector3 moveDirection = transform.position - previousPosition;
        if (moveDirection.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            waypointIndex++;
        }
    }

    void AnimateVisual()
    {
        if (visualTransform == null || data == null) return;

        float t = Time.time * data.bobSpeed;

        float offsetX = Mathf.Sin(t) * data.swayAmountX;
        float offsetY = Mathf.Abs(Mathf.Cos(t)) * data.bobAmountY;

        visualTransform.localPosition = visualBaseLocalPosition + new Vector3(offsetX, offsetY, 0f);
    }

    public void TakeDamage(float amount)
    {
        if (!isInitialized || isRemoved) return;
        if (data == null) return;
        if (amount <= 0f) return;

        if (currentShieldHealth > 0f)
        {
            currentShieldHealth -= amount;

            if (currentShieldHealth < 0f)
            {
                float remainingDamage = -currentShieldHealth;
                currentShieldHealth = 0f;
                currentHealth -= remainingDamage;
            }
        }
        else
        {
            currentHealth -= amount;
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

   void SpawnSplitChildren()
    {
        if (data == null) return;
        if (!data.splitsOnDeath) return;
        if (data.splitSpawnData == null) return;
        if (data.splitSpawnCount <= 0) return;
        if (WaveManager.Instance == null) return;

        int safeWaypointIndex = Mathf.Clamp(waypointIndex, 0, waypoints.Length - 1);

        for (int i = 0; i < data.splitSpawnCount; i++)
        {
            Vector3 offset = new Vector3((i == 0 ? -0.15f : 0.25f), 0f, 0f);
            Vector3 childSpawnPos = transform.position + offset;

            GameObject obj = Instantiate(WaveManager.Instance.enemyPrefab, childSpawnPos, Quaternion.identity);
            EnemyBase childEnemy = obj.GetComponent<EnemyBase>();

            if (childEnemy != null)
            {
                WaveManager.Instance.RegisterSpawnedEnemy();
                childEnemy.InitAtPoint(data.splitSpawnData, waypoints, childSpawnPos, safeWaypointIndex);
            }
        }
    }

    void Die()
    {
        if (isRemoved) return;
        isRemoved = true;

        SpawnSplitChildren();

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyRemoved();

        Destroy(gameObject);
    }

    void ReachedBase()
    {
        if (isRemoved) return;
        isRemoved = true;

        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyReachedBase();

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnEnemyRemoved();

        Destroy(gameObject);
    }
}