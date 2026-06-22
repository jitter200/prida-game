using System.Collections;
using UnityEngine;

public class HitSparkEffect : MonoBehaviour
{
    [Header("Sparks")]
    public int sparkCount = 10;
    public float lifeTime = 0.22f;
    public float minLength = 0.35f;
    public float maxLength = 1.2f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float lineWidth = 0.08f;

    [Header("Shape")]
    public float spreadAngle = 120f;
    public bool randomDirection = true;

    [Header("Colors")]
    public Color whiteSparkColor = new Color(1f, 0.95f, 0.85f, 1f);
    public Color orangeSparkColor = new Color(1f, 0.35f, 0f, 1f);

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 80;

    private LineRenderer[] lines;
    private Vector3[] directions;
    private float[] speeds;
    private float[] lengths;

    public static void Spawn(Vector3 position)
    {
        GameObject effectObject = new GameObject("HitSparkEffect");
        effectObject.transform.position = position;

        HitSparkEffect effect = effectObject.AddComponent<HitSparkEffect>();
        effect.Play(Vector2.zero);
    }

    public static void Spawn(Vector3 position, Vector2 hitDirection)
    {
        GameObject effectObject = new GameObject("HitSparkEffect");
        effectObject.transform.position = position;

        HitSparkEffect effect = effectObject.AddComponent<HitSparkEffect>();
        effect.Play(hitDirection);
    }

    public void Play(Vector2 hitDirection)
    {
        CreateSparks(hitDirection);
        StartCoroutine(EffectRoutine());
    }

    private void CreateSparks(Vector2 hitDirection)
    {
        lines = new LineRenderer[sparkCount];
        directions = new Vector3[sparkCount];
        speeds = new float[sparkCount];
        lengths = new float[sparkCount];

        Vector2 baseDirection = hitDirection.normalized;

        if (baseDirection == Vector2.zero || randomDirection)
        {
            baseDirection = Random.insideUnitCircle.normalized;
        }

        for (int i = 0; i < sparkCount; i++)
        {
            GameObject lineObject = new GameObject("SparkLine");
            lineObject.transform.SetParent(transform);
            lineObject.transform.localPosition = Vector3.zero;

            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            SetupLine(line, i);

            float angleOffset = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Vector2 dir = RotateVector(baseDirection, angleOffset).normalized;

            directions[i] = dir;
            speeds[i] = Random.Range(minSpeed, maxSpeed);
            lengths[i] = Random.Range(minLength, maxLength);

            lines[i] = line;
        }
    }

    private void SetupLine(LineRenderer line, int index)
    {
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = 0f;

        Shader shader = Shader.Find("Sprites/Default");

        if (shader != null)
        {
            line.material = new Material(shader);
        }

        Color color = index % 3 == 0 ? orangeSparkColor : whiteSparkColor;

        line.startColor = color;
        line.endColor = new Color(color.r, color.g, color.b, 0f);

        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;

        line.numCapVertices = 2;
    }

    private IEnumerator EffectRoutine()
    {
        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / lifeTime);
            float fade = 1f - progress;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == null) continue;

                Vector3 start = transform.position + directions[i] * speeds[i] * timer;
                Vector3 end = start + directions[i] * lengths[i] * fade;

                lines[i].SetPosition(0, start);
                lines[i].SetPosition(1, end);

                lines[i].startWidth = lineWidth * fade;
                lines[i].endWidth = 0f;

                Color startColor = i % 3 == 0 ? orangeSparkColor : whiteSparkColor;
                startColor.a = fade;

                Color endColor = startColor;
                endColor.a = 0f;

                lines[i].startColor = startColor;
                lines[i].endColor = endColor;
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
}