using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float lifeTime = 0.5f;

    private TextMeshPro textMesh;
    private Color startColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();

        if (textMesh != null)
            startColor = textMesh.color;
    }

    public void SetText(string message)
    {
        if (textMesh != null)
            textMesh.text = message;
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        lifeTime -= Time.deltaTime;

        if (textMesh != null)
        {
            float alpha = Mathf.Clamp01(lifeTime);
            textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        }

        if (lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}