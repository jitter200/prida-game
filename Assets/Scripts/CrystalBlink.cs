using UnityEngine;

public class CrystalBlink : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private SpriteRenderer baseSprite;
    [SerializeField] private SpriteRenderer glowSprite;

    [Header("Blink Settings")]
    [SerializeField] private float minAlpha = 0.15f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private float blinkSpeed = 1.5f;

    [Header("Scale Pulse")]
    [SerializeField] private bool useScalePulse = true;
    [SerializeField] private float scalePower = 0.05f;

    private Vector3 startScale;

    private void Start()
    {
        if (glowSprite != null)
        {
            startScale = glowSprite.transform.localScale;
        }
    }

    private void Update()
    {
        if (glowSprite == null) return;

        // Плавное значение от 0 до 1
        float t = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;

        // Прозрачность свечения
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color color = glowSprite.color;
        color.a = alpha;
        glowSprite.color = color;

        // Лёгкое увеличение/уменьшение свечения
        if (useScalePulse)
        {
            float scale = 1f + t * scalePower;
            glowSprite.transform.localScale = startScale * scale;
        }
    }
}