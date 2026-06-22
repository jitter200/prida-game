using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class RewindTrail : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifetime = 0.25f;

    [Header("Lines")]
    public int lineCount = 3;
    public float baseWidth = 0.12f;
    public float widthFalloff = 0.5f;
    public float verticalSpread = 0.06f;
    public float jitterAmplitude = 0.02f;

    [Header("Colors")]
    public Color headColor = new Color(1f, 0.95f, 0.3f, 1f);
    public Color tailColor = new Color(1f, 0.6f, 0.1f, 0f);

    [Header("Flash FX")]
    public GameObject flashPrefab;
    public float flashScale = 1f;
    public bool flashAtStart = true;
    public bool flashAtEnd = true;

    private readonly List<LineRenderer> lines = new List<LineRenderer>();
    private float dieAt;
    private Renderer sampleRendererForCopy;

    private void Awake()
    {
        LineRenderer rootLine = GetComponent<LineRenderer>();

        if (rootLine == null)
        {
            rootLine = gameObject.AddComponent<LineRenderer>();
        }

        PrepareLineRenderer(rootLine, baseWidth);

        lines.Add(rootLine);

        sampleRendererForCopy = rootLine;
    }

    public void Set(Vector3 startPosition, Vector3 endPosition, float width, float life)
    {
        baseWidth = width;
        lifetime = life;
        dieAt = Time.unscaledTime + lifetime;

        EnsureChildLines();

        for (int i = 0; i < lines.Count; i++)
        {
            LineRenderer lineRenderer = lines[i];

            float t = lines.Count == 1
                ? 0f
                : Mathf.Lerp(-1f, 1f, i / (float)(lines.Count - 1));

            float yOffset = t * verticalSpread;

            float currentWidth = baseWidth * Mathf.Lerp(
                1f,
                widthFalloff,
                Mathf.Abs(t)
            );

            Vector3 pointA = new Vector3(
                startPosition.x,
                startPosition.y + yOffset,
                0f
            );

            Vector3 pointB = new Vector3(
                endPosition.x,
                endPosition.y + yOffset,
                0f
            );

            float jitterA = (Random.value - 0.5f) * jitterAmplitude;
            float jitterB = (Random.value - 0.5f) * jitterAmplitude;

            pointA.y += jitterA;
            pointB.y += jitterB;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, pointA);
            lineRenderer.SetPosition(1, pointB);

            lineRenderer.startWidth = currentWidth;
            lineRenderer.endWidth = 0f;

            Gradient gradient = new Gradient();

            gradient.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(headColor, 0f),
                    new GradientColorKey(headColor, 1f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(headColor.a, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );

            lineRenderer.colorGradient = gradient;
        }

        if (flashPrefab != null)
        {
            if (flashAtStart)
            {
                SpawnFlash(startPosition);
            }

            if (flashAtEnd)
            {
                SpawnFlash(endPosition);
            }
        }
    }

    private void Update()
    {
        float t = Mathf.InverseLerp(
            dieAt,
            dieAt - lifetime,
            Time.unscaledTime
        );

        foreach (LineRenderer lineRenderer in lines)
        {
            if (lineRenderer == null) continue;

            Gradient gradient = lineRenderer.colorGradient;

            GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

            alphaKeys[0].alpha = Mathf.Lerp(1f, 0f, t * 0.9f);
            alphaKeys[1].alpha = 0f;

            gradient.alphaKeys = alphaKeys;

            lineRenderer.colorGradient = gradient;
        }

        if (Time.unscaledTime >= dieAt)
        {
            Destroy(gameObject);
        }
    }

    private void EnsureChildLines()
    {
        int targetLineCount = Mathf.Max(1, lineCount);

        for (int i = lines.Count; i < targetLineCount; i++)
        {
            GameObject child = new GameObject("TrailLine_" + i);

            child.transform.SetParent(transform, true);

            LineRenderer lineRenderer = child.AddComponent<LineRenderer>();

            CopyRendererSettings(sampleRendererForCopy, lineRenderer);

            PrepareLineRenderer(lineRenderer, baseWidth);

            lines.Add(lineRenderer);
        }

        while (lines.Count > targetLineCount)
        {
            LineRenderer lineRenderer = lines[lines.Count - 1];

            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }

            lines.RemoveAt(lines.Count - 1);
        }
    }

    private void PrepareLineRenderer(LineRenderer lineRenderer, float width)
    {
        lineRenderer.useWorldSpace = true;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.numCornerVertices = 3;
        lineRenderer.numCapVertices = 3;
        lineRenderer.textureMode = LineTextureMode.Stretch;

        if (lineRenderer.sharedMaterial == null)
        {
            lineRenderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = 0f;

        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(headColor, 0f),
                new GradientColorKey(headColor, 1f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(headColor.a, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );

        lineRenderer.colorGradient = gradient;
    }

    private void CopyRendererSettings(Renderer from, Renderer to)
    {
        if (from == null || to == null) return;

        to.sortingLayerID = from.sortingLayerID;
        to.sortingLayerName = from.sortingLayerName;
        to.sortingOrder = from.sortingOrder + 10;
        to.renderingLayerMask = from.renderingLayerMask;

        if (from.sharedMaterial != null)
        {
            to.sharedMaterial = from.sharedMaterial;
        }

        to.gameObject.layer = from.gameObject.layer;
    }

    private void SpawnFlash(Vector3 position)
    {
        GameObject flash = Instantiate(flashPrefab, position, Quaternion.identity);

        flash.transform.localScale = Vector3.one * flashScale;

        LineRenderer sourceLine = lines.Count > 0 ? lines[0] : GetComponent<LineRenderer>();

        SpriteRenderer[] spriteRenderers = flash.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingLayerID = sourceLine.sortingLayerID;
            spriteRenderer.sortingLayerName = sourceLine.sortingLayerName;
            spriteRenderer.sortingOrder = sourceLine.sortingOrder + 1;
            spriteRenderer.renderingLayerMask = sourceLine.renderingLayerMask;

            flash.layer = gameObject.layer;
        }

        float life = 0.3f;

        Animator animator = flash.GetComponentInChildren<Animator>();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            AnimatorClipInfo[] clipInfos = animator.GetCurrentAnimatorClipInfo(0);

            if (clipInfos != null && clipInfos.Length > 0 && clipInfos[0].clip != null)
            {
                life = Mathf.Max(life, clipInfos[0].clip.length + 0.05f);
            }
        }

        ParticleSystem particleSystem = flash.GetComponentInChildren<ParticleSystem>();

        if (particleSystem != null)
        {
            life = Mathf.Max(
                life,
                particleSystem.main.duration + particleSystem.main.startLifetime.constantMax + 0.05f
            );
        }

        Destroy(flash, life);
    }
}