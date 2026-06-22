using System.Collections;
using UnityEngine;

public class LightningStrikeFX : MonoBehaviour
{
    [Header("Main Bolt")]
    public float heightAboveTarget = 8f;
    public int segments = 8;
    public float zigzagAmount = 0.35f;
    public float boltWidth = 0.12f;
    public float lifetime = 0.25f;

    [Header("Branches")]
    public int branchCount = 4;
    public float branchLength = 1.2f;
    public float branchWidth = 0.05f;

    [Header("Impact Ring")]
    public bool useImpactRing = true;
    public float impactRadius = 2.5f;
    public int circlePoints = 96;
    public float ringWidth = 0.08f;

    [Header("Visual")]
    public Color startColor = new Color(0.4f, 0.9f, 1f, 1f);
    public Color endColor = new Color(0.8f, 1f, 1f, 0f);
    public string sortingLayerName = "Default";
    public int sortingOrder = 30;

    private Material lineMaterial;

    public void Play(Vector3 targetPosition, float radius)
    {
        impactRadius = radius;

        lineMaterial = new Material(Shader.Find("Sprites/Default"));

        Vector3 startPosition = targetPosition + Vector3.up * heightAboveTarget;

        LineRenderer mainBolt = CreateLine("Main_Lightning_Bolt", boltWidth);
        DrawZigzagBolt(mainBolt, startPosition, targetPosition);

        for (int i = 0; i < branchCount; i++)
        {
            CreateBranch(mainBolt);
        }

        if (useImpactRing)
        {
            StartCoroutine(PlayImpactRing(targetPosition));
        }

        StartCoroutine(FadeAndDestroy());
    }

    private LineRenderer CreateLine(string objectName, float width)
    {
        GameObject lineObject = new GameObject(objectName);
        lineObject.transform.SetParent(transform);
        lineObject.transform.localPosition = Vector3.zero;

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.alignment = LineAlignment.View;

        line.material = lineMaterial;

        line.startWidth = width;
        line.endWidth = width;

        line.startColor = startColor;
        line.endColor = startColor;

        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        line.numCornerVertices = 2;
        line.numCapVertices = 2;

        return line;
    }

    private void DrawZigzagBolt(LineRenderer line, Vector3 startPosition, Vector3 endPosition)
    {
        line.positionCount = segments + 1;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;

            Vector3 point = Vector3.Lerp(startPosition, endPosition, t);

            if (i != 0 && i != segments)
            {
                point.x += Random.Range(-zigzagAmount, zigzagAmount);
                point.y += Random.Range(-zigzagAmount * 0.5f, zigzagAmount * 0.5f);
            }

            point.z = 0f;

            line.SetPosition(i, point);
        }
    }

    private void CreateBranch(LineRenderer mainBolt)
    {
        if (mainBolt.positionCount < 3) return;

        int index = Random.Range(1, mainBolt.positionCount - 1);

        Vector3 start = mainBolt.GetPosition(index);

        Vector2 randomDirection = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-0.4f, 0.6f)
        ).normalized;

        Vector3 end = start + (Vector3)(randomDirection * branchLength);
        end.z = 0f;

        LineRenderer branch = CreateLine("Lightning_Branch", branchWidth);

        branch.positionCount = 3;

        Vector3 middle = Vector3.Lerp(start, end, 0.5f);
        middle.x += Random.Range(-zigzagAmount, zigzagAmount);
        middle.y += Random.Range(-zigzagAmount, zigzagAmount);

        branch.SetPosition(0, start);
        branch.SetPosition(1, middle);
        branch.SetPosition(2, end);
    }

    private IEnumerator PlayImpactRing(Vector3 centerPosition)
    {
        LineRenderer ring = CreateLine("Lightning_Impact_Ring", ringWidth);

        ring.loop = true;
        ring.positionCount = circlePoints;

        float timer = 0f;
        float ringDuration = lifetime;

        while (timer < ringDuration)
        {
            timer += Time.deltaTime;

            float t = timer / ringDuration;

            float currentRadius = Mathf.Lerp(0.2f, impactRadius, t);

            Color color = Color.Lerp(startColor, endColor, t);

            ring.startColor = color;
            ring.endColor = color;

            ring.startWidth = Mathf.Lerp(ringWidth, 0.01f, t);
            ring.endWidth = Mathf.Lerp(ringWidth, 0.01f, t);

            DrawCircle(ring, centerPosition, currentRadius);

            yield return null;
        }
    }

    private void DrawCircle(LineRenderer line, Vector3 centerPosition, float radius)
    {
        for (int i = 0; i < circlePoints; i++)
        {
            float angle = i * Mathf.PI * 2f / circlePoints;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            Vector3 point = new Vector3(
                centerPosition.x + x,
                centerPosition.y + y,
                0f
            );

            line.SetPosition(i, point);
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        float timer = 0f;

        LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();

        while (timer < lifetime)
        {
            timer += Time.deltaTime;

            float t = timer / lifetime;

            Color color = Color.Lerp(startColor, endColor, t);

            foreach (LineRenderer line in lines)
            {
                if (line == null) continue;

                line.startColor = color;
                line.endColor = color;

                line.startWidth = Mathf.Lerp(line.startWidth, 0.01f, t);
                line.endWidth = Mathf.Lerp(line.endWidth, 0.01f, t);
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}