using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float moveSpeed;

    public Sprite enemySprite;
    public Vector3 visualScale = Vector3.one;
    public Color enemyColor = Color.white;

    [Header("Shield")]
    public bool hasShield = false;
    public float shieldHealth = 0f;

    [Header("Splitter")]
    public bool splitsOnDeath = false;
    public EnemyData splitSpawnData;
    public int splitSpawnCount = 0;

    [Header("Visual Motion")]
    public float bobSpeed = 8f;
    public float bobAmountY = 0.05f;
    public float swayAmountX = 0.03f;
}