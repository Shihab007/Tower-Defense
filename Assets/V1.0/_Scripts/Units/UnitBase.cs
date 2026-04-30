using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public UnitData data;
    public Vector2Int currentCell;
    public GameObject bulletPrefab;

    private float attackTimer = 0f;
    private EnemyBase currentTarget;
    private SpriteRenderer spriteRenderer;
    private Transform visualTransform;

    void Awake()
    {
        visualTransform = transform.Find("Visual");

        if (visualTransform != null)
            spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
        else
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (data == null) return;

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

        // Keep root scale at 1 so collider stays usable
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
        if (bulletPrefab == null) return;

        GameObject obj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        BulletBase bullet = obj.GetComponent<BulletBase>();
        bullet.Init(target, data.damage);
    }
}