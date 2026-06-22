using UnityEngine;

public class CB : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer baseSprite;
    [SerializeField] private SpriteRenderer glowSprite1;
    [SerializeField] private SpriteRenderer glowSprite2;

    [Header("Blink Settings")]
    [SerializeField] private float minAlpha = 0.15f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float blinkSpeed = 1.5f;

    [Header("Second Glow Offset")]
    [SerializeField] private float secondGlowOffset = 0.7f;

    [Header("Scale Pulse")]
    [SerializeField] private bool useScalePulse = true;
    [SerializeField] private float scalePower = 0.05f;

    private Vector3 startScale1;
    private Vector3 startScale2;

    private void Start()
    {
        if (glowSprite1 != null)
            startScale1 = glowSprite1.transform.localScale;

        if (glowSprite2 != null)
            startScale2 = glowSprite2.transform.localScale;
    }

    private void Update()
    {
        BlinkGlow(glowSprite1, startScale1, 0f);
        BlinkGlow(glowSprite2, startScale2, secondGlowOffset);
    }

    private void BlinkGlow(SpriteRenderer glowSprite, Vector3 startScale, float offset)
    {
        if (glowSprite == null) return;

        float t = (Mathf.Sin((Time.unscaledTime + offset) * blinkSpeed) + 1f) / 2f;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color color = glowSprite.color;
        color.a = alpha;
        glowSprite.color = color;

        if (useScalePulse)
        {
            float scale = 1f + t * scalePower;
            glowSprite.transform.localScale = startScale * scale;
        }
    }
}