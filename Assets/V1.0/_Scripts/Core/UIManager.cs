using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public GameObject gameOverPanel;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver) return;
        timerText.text = $"{GameManager.Instance.survivalTime:F1}s";
    }

    public void UpdateWave(int wave)
    {
        waveText.text = $"Wave {wave}";
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"Lives: {lives}";
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}