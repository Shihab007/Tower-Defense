using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public int tier;
    public float damage;
    public float attackSpeed;
    public float range;
    public int summonCost;
    public GameObject prefab;
    public Sprite icon;

    public Sprite unitSprite;
    public Vector3 visualScale = Vector3.one;
    public Color unitColor = Color.white;
    public UnitData nextTierUnit;

    [Header("Economy")]
    public bool generatesMana = false;
    public int manaPerTick = 0;
    public float manaTickInterval = 5f;

    [Header("Control")]
    public bool appliesSlow = false;
    public float slowMultiplier = 1f;
    public float slowDuration = 0f;

    [Header("Freeze")]
    public bool appliesChill = false;
    public int chillPerHit = 0;
    public int freezeThreshold = 0;
    public float freezeDuration = 0f;
}