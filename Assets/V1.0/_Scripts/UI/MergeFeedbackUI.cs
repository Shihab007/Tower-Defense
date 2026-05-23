using UnityEngine;
using TMPro;
using System.Collections;

public class MergeFeedbackUI : MonoBehaviour
{
    public static MergeFeedbackUI Instance;

    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private float visibleDuration = 1f;

    private Coroutine currentRoutine;

    void Awake()
    {
        Instance = this;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    public void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    IEnumerator ShowRoutine(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        Color startColor = feedbackText.color;
        feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, 1f);

        float timer = 0f;

        while (timer < visibleDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / visibleDuration);
            feedbackText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        feedbackText.gameObject.SetActive(false);
        feedbackText.color = startColor;
        currentRoutine = null;
    }
}