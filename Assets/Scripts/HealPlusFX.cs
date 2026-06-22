using System.Collections;
using UnityEngine;

public class HealPlusFX : MonoBehaviour
{
    [Header("Plus Settings")]
    public int plusCount = 10;
    public float spawnRadiusX = 1.2f;
    public float spawnRadiusY = 1.0f;
    public float plusSize = 0.25f;
    public float lineWidth = 0.06f;

    [Header("Animation")]
    public float duration = 0.8f;
    public float riseDistance = 1.0f;
    public float spawnDelay = 0.04f;

    [Header("Visual")]
    public Color startColor = new Color(0.2f, 1f, 0.35f, 1f);
    public Color endColor = new Color(0.2f, 1f, 0.35f, 0f);
    public string sortingLayerName = "Default";
    public int sortingOrder = 30;

    private void Start()
    {
        StartCoroutine(PlayFX());
    }

    private IEnumerator PlayFX()
    {
        for (int i = 0; i < plusCount; i++)
        {
            SpawnPlus();
            yield return new WaitForSeconds(spawnDelay);
        }

        Destroy(gameObject, duration + spawnDelay * plusCount + 0.2f);
    }

    private void SpawnPlus()
    {
        GameObject plusObject = new GameObject("Heal_Plus");
        plusObject.transform.SetParent(transform);

        Vector3 randomLocalPosition = new Vector3(
            Random.Range(-spawnRadiusX, spawnRadiusX),
            Random.Range(-spawnRadiusY, spawnRadiusY),
            0f
        );

        plusObject.transform.localPosition = randomLocalPosition;

        LineRenderer horizontalLine = CreateLine("Horizontal_Line", plusObject.transform);
        LineRenderer verticalLine = CreateLine("Vertical_Line", plusObject.transform);

        horizontalLine.positionCount = 2;
        horizontalLine.SetPosition(0, new Vector3(-plusSize, 0f, 0f));
        horizontalLine.SetPosition(1, new Vector3(plusSize, 0f, 0f));

        verticalLine.positionCount = 2;
        verticalLine.SetPosition(0, new Vector3(0f, -plusSize, 0f));
        verticalLine.SetPosition(1, new Vector3(0f, plusSize, 0f));

        StartCoroutine(AnimatePlus(plusObject, horizontalLine, verticalLine));
    }

    private LineRenderer CreateLine(string lineName, Transform parent)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(parent);
        lineObject.transform.localPosition = Vector3.zero;

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = false;
        line.alignment = LineAlignment.View;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.startColor = startColor;
        line.endColor = startColor;

        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        line.numCornerVertices = 2;
        line.numCapVertices = 2;

        line.material = new Material(Shader.Find("Sprites/Default"));

        return line;
    }

    private IEnumerator AnimatePlus(GameObject plusObject, LineRenderer horizontalLine, LineRenderer verticalLine)
    {
        float timer = 0f;

        Vector3 startPosition = plusObject.transform.localPosition;
        Vector3 endPosition = startPosition + new Vector3(0f, riseDistance, 0f);

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;

            plusObject.transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            Color currentColor = Color.Lerp(startColor, endColor, t);

            SetLineColor(horizontalLine, currentColor);
            SetLineColor(verticalLine, currentColor);

            float currentWidth = Mathf.Lerp(lineWidth, 0.01f, t);

            horizontalLine.startWidth = currentWidth;
            horizontalLine.endWidth = currentWidth;

            verticalLine.startWidth = currentWidth;
            verticalLine.endWidth = currentWidth;

            yield return null;
        }

        Destroy(plusObject);
    }

    private void SetLineColor(LineRenderer line, Color color)
    {
        if (line == null) return;

        line.startColor = color;
        line.endColor = color;
    }
}