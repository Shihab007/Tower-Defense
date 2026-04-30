using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public EnemyData data;

    private float currentHealth;
    private int waypointIndex = 0;
    private Transform[] waypoints;
    private bool isInitialized = false;
    private bool isRemoved = false;

    private Transform visualTransform;
    private SpriteRenderer spriteRenderer;
    private Vector3 visualBaseLocalPosition;

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

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("EnemyBase: No waypoints assigned.");
            return;
        }

        ApplyVisuals();

        transform.position = waypoints[0].position;
        waypointIndex = 1;
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

        Move();
        AnimateVisual();
    }

    void Move()
    {
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
            data.moveSpeed * Time.deltaTime
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

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isRemoved) return;
        isRemoved = true;

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