using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndingSlideshowUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Image slideImage;

    [Header("Slides")]
    public Sprite[] killEndingSlides;
    public Sprite[] spareEndingSlides;

    [Header("Timing")]
    public float slideDuration = 3f;
    public float fadeTime = 0.5f;

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    private Coroutine slideshowRoutine;

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        SetImageAlpha(0f);
    }

    public void PlayEnding(bool killEnding)
    {
        Time.timeScale = 1f;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (slideshowRoutine != null)
        {
            StopCoroutine(slideshowRoutine);
        }

        Sprite[] slides = killEnding ? killEndingSlides : spareEndingSlides;

        slideshowRoutine = StartCoroutine(SlideshowRoutine(slides));
    }

    private IEnumerator SlideshowRoutine(Sprite[] slides)
    {
        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("Ending slides are empty");
            SceneManager.LoadScene(mainMenuSceneName);
            yield break;
        }

        for (int i = 0; i < slides.Length; i++)
        {
            if (slideImage != null)
            {
                slideImage.sprite = slides[i];
            }

            yield return StartCoroutine(FadeImage(0f, 1f));
            yield return new WaitForSecondsRealtime(slideDuration);
            yield return StartCoroutine(FadeImage(1f, 0f));
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadeImage(float fromAlpha, float toAlpha)
    {
        if (slideImage == null) yield break;

        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / fadeTime);
            SetImageAlpha(Mathf.Lerp(fromAlpha, toAlpha, t));

            yield return null;
        }

        SetImageAlpha(toAlpha);
    }

    private void SetImageAlpha(float alpha)
    {
        if (slideImage == null) return;

        Color color = slideImage.color;
        color.a = alpha;
        slideImage.color = color;
    }
}