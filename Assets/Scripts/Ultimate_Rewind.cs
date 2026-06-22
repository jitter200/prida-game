using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultimate_Rewind : MonoBehaviour
{
    [Header("Settings")]
    public float rewindSeconds = 3f;
    public float cooldown = 10f;

    [Header("FX")]
    public SpriteRenderer playerSpriteRenderer;
    public GameObject rewindStartFX;
    public GameObject rewindEndFX;
    public GameObject teleportTrailPrefab;

    [Header("Trail")]
    public float trailWidth = 0.12f;
    public float trailLifetime = 0.25f;

    [Header("Afterimages")]
    public bool useAfterimages = true;
    public int ghostsCount = 5;
    public float ghostLifetime = 0.35f;
    public Color ghostColor = new Color(1f, 1f, 1f, 0.5f);

    [Header("Slow Motion")]
    public bool useSlowMotion = true;
    public float slowMotionScale = 0.2f;
    public float slowMotionDuration = 0.15f;

    private float lastUseTime = -999f;
    private Rigidbody2D rb;

    private float defaultFixedDeltaTime;

    private class Snapshot
    {
        public Vector3 position;
        public Vector2 velocity;
        public float time;

        public Snapshot(Vector3 position, Vector2 velocity, float time)
        {
            this.position = position;
            this.velocity = velocity;
            this.time = time;
        }
    }

    private readonly List<Snapshot> history = new List<Snapshot>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultFixedDeltaTime = Time.fixedDeltaTime;

        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void FixedUpdate()
    {
        Record();
    }

    private void Record()
    {
        if (rb == null) return;

        float currentTime = Time.time;

        history.Add(new Snapshot(transform.position, rb.velocity, currentTime));

        float cutoffTime = currentTime - rewindSeconds - 0.2f;
        history.RemoveAll(snapshot => snapshot.time < cutoffTime);
    }

    public void Use()
    {
        if (!CanUse()) return;

        StartCoroutine(RewindRoutine());
    }

    private IEnumerator RewindRoutine()
    {
        lastUseTime = Time.time;

        Vector3 startPosition = transform.position;

        if (useSlowMotion)
        {
            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
            yield return new WaitForSecondsRealtime(slowMotionDuration);
        }

        Snapshot targetSnapshot = GetTargetSnapshot();

        Vector3 endPosition = targetSnapshot.position;

        SpawnFX(rewindStartFX, startPosition);

        if (rb != null)
        {
            rb.velocity = targetSnapshot.velocity;
            rb.position = targetSnapshot.position;
        }

        transform.position = targetSnapshot.position;

        SpawnTrail(startPosition, endPosition);

        if (useAfterimages)
        {
            SpawnAfterimages(startPosition, endPosition);
        }

        SpawnFX(rewindEndFX, endPosition);

        if (useSlowMotion)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = defaultFixedDeltaTime;
        }

        Debug.Log("Rewind ultimate used");
    }

    private Snapshot GetTargetSnapshot()
    {
        float targetTime = Time.time - rewindSeconds;

        Snapshot bestSnapshot = history[0];

        foreach (Snapshot snapshot in history)
        {
            if (snapshot.time <= targetTime)
            {
                bestSnapshot = snapshot;
            }
            else
            {
                break;
            }
        }

        return bestSnapshot;
    }

    private bool CanUse()
    {
        if (Time.time < lastUseTime + cooldown)
        {
            Debug.Log("Rewind cooldown");
            return false;
        }

        if (history.Count == 0)
        {
            Debug.Log("No rewind history");
            return false;
        }

        return true;
    }

    private void SpawnTrail(Vector3 fromPosition, Vector3 toPosition)
    {
        if (teleportTrailPrefab == null) return;

        GameObject trailObject = Instantiate(teleportTrailPrefab);

        RewindTrail trail = trailObject.GetComponent<RewindTrail>();

        if (trail != null)
        {
            trail.Set(fromPosition, toPosition, trailWidth, trailLifetime);
        }
        else
        {
            Destroy(trailObject);
        }
    }

    private void SpawnFX(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return;

        GameObject fx = Instantiate(prefab, position, Quaternion.identity);

        Destroy(fx, 1f);
    }

    private void SpawnAfterimages(Vector3 fromPosition, Vector3 toPosition)
    {
        if (playerSpriteRenderer == null) return;
        if (playerSpriteRenderer.sprite == null) return;

        for (int i = 0; i < ghostsCount; i++)
        {
            float t = (i + 1f) / (ghostsCount + 1f);

            Vector3 position = Vector3.Lerp(fromPosition, toPosition, t);

            GameObject ghost = new GameObject("Rewind_Ghost");

            ghost.transform.position = position;
            ghost.transform.localScale = transform.localScale;

            SpriteRenderer ghostRenderer = ghost.AddComponent<SpriteRenderer>();

            ghostRenderer.sprite = playerSpriteRenderer.sprite;
            ghostRenderer.flipX = playerSpriteRenderer.flipX;
            ghostRenderer.sortingLayerName = playerSpriteRenderer.sortingLayerName;
            ghostRenderer.sortingOrder = playerSpriteRenderer.sortingOrder + 5;

            Color color = ghostColor;
            color.a *= 1f - t;
            ghostRenderer.color = color;

            StartCoroutine(FadeAndDestroyGhost(ghostRenderer, ghostLifetime));
        }
    }

    private IEnumerator FadeAndDestroyGhost(SpriteRenderer ghostRenderer, float lifetime)
    {
        float timer = 0f;

        Color startColor = ghostRenderer.color;

        while (timer < lifetime)
        {
            timer += Time.unscaledDeltaTime;

            float t = timer / lifetime;

            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, t);

            ghostRenderer.color = color;

            yield return null;
        }

        if (ghostRenderer != null)
        {
            Destroy(ghostRenderer.gameObject);
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, lastUseTime + cooldown - Time.time);
    }

    public float GetCooldownDuration()
    {
        return cooldown;
    }
}