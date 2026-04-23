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
}