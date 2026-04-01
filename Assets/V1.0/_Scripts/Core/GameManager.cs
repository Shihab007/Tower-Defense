using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int lives = 3;
    public float survivalTime = 0f;
    public bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isGameOver) return;
        survivalTime += Time.deltaTime;
    }

    public void OnEnemyReachedBase()
{
    if (isGameOver) return;
    lives--;
    UIManager.Instance.UpdateLives(lives);
    Debug.Log($"Life lost. Lives remaining: {lives}");
    if (lives <= 0) TriggerGameOver();
}

void TriggerGameOver()
{
    isGameOver = true;
    UIManager.Instance.ShowGameOver();
    Debug.Log($"Game Over. Survived: {survivalTime:F1} seconds");
}
}