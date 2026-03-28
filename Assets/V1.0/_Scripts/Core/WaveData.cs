using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Game/Wave Data")]
public class WaveData : ScriptableObject
{
    public EnemyData enemyType;
    public int enemyCount;
    public float spawnInterval;
}