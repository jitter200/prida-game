using System.Collections;
using UnityEngine;

public class HealAuraFX : MonoBehaviour
{
    [Header("Aura")]
    public float radius = 1.2f;
    public float duration = 0.8f;
    public int ringCount = 3;
    public float ringDelay = 0.15f;

    [Header("Line Settings")]
    public int points = 96;
    public float startWidth = 0.08f;
    public float endWidth = 0.01f;

    [Header("Movement")]
    public float startScale = 0.4f;
    public float endScale = 1.4f;
    public float verticalRise = 0.8f;

    [Header("Visual")]
    public Color startColor = new Color(0.3f, 1f, 0.5f, 1f);
    public Color endColor = new Color(0.3f, 1f, 0.5f, 0f);
    public string sortingLayerName = "Default";
    public int sortingOrder = 30;

    private void Start()
    {
        StartCoroutine(PlayFX());
    }

    private IEnumerator PlayFX()
    {
        for (int i = 0; i < ringCount; i++)
        {
            StartCoroutine(PlayRing(i));
            yield return new WaitForSeconds(ringDelay);
        }

        Destroy(gameObject, duration + ringDelay * ringCount + 0.2f);
    }

    private IEnumerator PlayRing(int index)
    {
        GameObject ringObject = new GameObject("Heal_Ring_" + index);
        ringObject.transform.SetParent(transform);
        ringObject.transform.localPosition = Vector3.zero;

        LineRenderer line = ringObject.AddComponent<LineRenderer>();

        SetupLineRenderer(line);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            float currentScale = Mathf.Lerp(startScale, endScale, t);
            float currentRadius = radius * currentScale;

            float yOffset = Mathf.Lerp(0f, verticalRise, t);

            float currentWidth = Mathf.Lerp(startWidth, endWidth, t);

            Color currentColor = Color.Lerp(startColor, endColor, t);

            line.startWidth = currentWidth;
            line.endWidth = currentWidth;
            line.startColor = currentColor;
            line.endColor = currentColor;

            DrawCircle(line, currentRadius, yOffset);

            yield return null;
        }

        Destroy(ringObject);
    }

    private void SetupLineRenderer(LineRenderer line)
    {
        line.useWorldSpace = false;
        line.loop = true;
        line.positionCount = points;

        line.alignment = LineAlignment.View;
        line.numCornerVertices = 4;
        line.numCapVertices = 4;

        line.startWidth = startWidth;
        line.endWidth = startWidth;

        line.startColor = startColor;
        line.endColor = startColor;

        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        line.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void DrawCircle(LineRenderer line, float currentRadius, float yOffset)
    {
        for (int i = 0; i < points; i++)
        {
            float angle = i * Mathf.PI * 2f / points;

            float x = Mathf.Cos(angle) * currentRadius;
            float y = Mathf.Sin(angle) * currentRadius * 0.35f + yOffset;

            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}