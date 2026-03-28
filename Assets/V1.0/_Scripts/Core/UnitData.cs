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
}