using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float maxRayDistance = 8f;
    public float yOffset = 0.03f;

    [Header("Shadow Size")]
    public Vector2 baseScale = new Vector2(1.2f, 0.35f);
    public float minScale = 0.45f;
    public float maxScale = 1f;

    [Header("Shadow Alpha")]
    public float minAlpha = 0.12f;
    public float maxAlpha = 0.45f;

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 5;

    private Transform shadowTransform;
    private SpriteRenderer shadowRenderer;

    private void Awake()
    {
        CreateShadow();
    }

    private void LateUpdate()
    {
        UpdateShadow();
    }

    private void CreateShadow()
    {
        GameObject shadowObject = new GameObject("PlayerShadow");
        shadowTransform = shadowObject.transform;

        shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = CreateOvalSprite(128, 64);
        shadowRenderer.sortingLayerName = sortingLayerName;
        shadowRenderer.sortingOrder = sortingOrder;

        Color color = Color.black;
        color.a = maxAlpha;
        shadowRenderer.color = color;
    }

    private void UpdateShadow()
    {
        if (shadowTransform == null || shadowRenderer == null)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            maxRayDistance,
            groundLayer
        );

        if (hit.collider == null)
        {
            shadowRenderer.enabled = false;
            return;
        }

        shadowRenderer.enabled = true;

        Vector3 shadowPosition = new Vector3(
            transform.position.x,
            hit.point.y + yOffset,
            transform.position.z
        );

        shadowTransform.position = shadowPosition;

        float distanceToGround = Mathf.Abs(transform.position.y - hit.point.y);
        float t = Mathf.Clamp01(distanceToGround / maxRayDistance);

        float scale = Mathf.Lerp(maxScale, minScale, t);
        float alpha = Mathf.Lerp(maxAlpha, minAlpha, t);

        shadowTransform.localScale = new Vector3(
            baseScale.x * scale,
            baseScale.y * scale,
            1f
        );

        Color color = Color.black;
        color.a = alpha;
        shadowRenderer.color = color;

        shadowRenderer.sortingLayerName = sortingLayerName;
        shadowRenderer.sortingOrder = sortingOrder;
    }

    private Sprite CreateOvalSprite(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (x / (float)(width - 1)) * 2f - 1f;
                float ny = (y / (float)(height - 1)) * 2f - 1f;

                float distance = nx * nx + ny * ny;

                if (distance <= 1f)
                {
                    float alpha = Mathf.Pow(1f - distance, 1.5f);
                    texture.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    private void OnDestroy()
    {
        if (shadowTransform != null)
        {
            Destroy(shadowTransform.gameObject);
        }
    }
}