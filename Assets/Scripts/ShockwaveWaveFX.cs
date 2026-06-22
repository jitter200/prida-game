using System.Collections;
using UnityEngine;

public class ShockwaveWaveFX : MonoBehaviour
{
    [Header("Wave")]
    public float targetRadius = 5f;
    public float duration = 0.35f;
    public int ringCount = 3;
    public float ringDelay = 0.08f;

    [Header("Circle")]
    public int points = 96;
    public float startWidth = 0.12f;
    public float endWidth = 0.02f;

    [Header("Visual")]
    public Color startColor = new Color(0.3f, 1f, 1f, 1f);
    public Color endColor = new Color(0.3f, 1f, 1f, 0f);
    public string sortingLayerName = "Default";
    public int sortingOrder = 20;

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

        float totalLife = duration + ringDelay * ringCount + 0.1f;
        Destroy(gameObject, totalLife);
    }

    private IEnumerator PlayRing(int index)
    {
        GameObject ringObject = new GameObject("Shockwave_Ring_" + index);
        ringObject.transform.SetParent(transform);
        ringObject.transform.localPosition = Vector3.zero;

        LineRenderer line = ringObject.AddComponent<LineRenderer>();

        SetupLineRenderer(line);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            float radius = Mathf.Lerp(0.1f, targetRadius, t);

            float width = Mathf.Lerp(startWidth, endWidth, t);

            Color color = Color.Lerp(startColor, endColor, t);

            line.startWidth = width;
            line.endWidth = width;
            line.startColor = color;
            line.endColor = color;

            DrawCircle(line, radius);

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

        Material material = new Material(Shader.Find("Sprites/Default"));
        line.material = material;
    }

    private void DrawCircle(LineRenderer line, float radius)
    {
        for (int i = 0; i < points; i++)
        {
            float angle = i * Mathf.PI * 2f / points;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            line.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}