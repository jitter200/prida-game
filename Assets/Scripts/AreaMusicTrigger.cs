using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaMusicTrigger : MonoBehaviour
{
    [Header("Music")]
    public AudioSource areaMusicSource;
    public AudioClip areaMusicClip;

    [Header("Settings")]
    public bool playOnlyOnce = false;
    public bool stopWhenExit = true;
    public bool loop = true;

    [Range(0f, 1f)]
    public float targetVolume = 0.4f;

    public float fadeInTime = 1.5f;
    public float fadeOutTime = 1.5f;

    private bool hasPlayed;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        Collider2D trigger = GetComponent<Collider2D>();
        trigger.isTrigger = true;

        if (areaMusicSource == null)
        {
            areaMusicSource = GetComponent<AudioSource>();
        }

        if (areaMusicSource != null)
        {
            areaMusicSource.playOnAwake = false;
            areaMusicSource.loop = loop;
            areaMusicSource.spatialBlend = 0f;
            areaMusicSource.volume = 0f;

            if (areaMusicClip != null)
            {
                areaMusicSource.clip = areaMusicClip;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (playOnlyOnce && hasPlayed) return;

        hasPlayed = true;

        StartAreaMusic();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (stopWhenExit)
        {
            StopAreaMusic();
        }
    }

    private void StartAreaMusic()
    {
        if (areaMusicSource == null) return;

        if (areaMusicClip != null && areaMusicSource.clip != areaMusicClip)
        {
            areaMusicSource.clip = areaMusicClip;
        }

        areaMusicSource.loop = loop;

        if (!areaMusicSource.isPlaying)
        {
            areaMusicSource.volume = 0f;
            areaMusicSource.Play();
        }

        StartFade(targetVolume, fadeInTime);
    }

    private void StopAreaMusic()
    {
        if (areaMusicSource == null) return;

        StartFade(0f, fadeOutTime, true);
    }

    private void StartFade(float target, float duration, bool stopAfterFade = false)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeRoutine(target, duration, stopAfterFade));
    }

    private IEnumerator FadeRoutine(float target, float duration, bool stopAfterFade)
    {
        float startVolume = areaMusicSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            areaMusicSource.volume = Mathf.Lerp(startVolume, target, t);

            yield return null;
        }

        areaMusicSource.volume = target;

        if (stopAfterFade)
        {
            areaMusicSource.Stop();
        }
    }
}