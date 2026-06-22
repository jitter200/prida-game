using System.Collections;
using UnityEngine;

public class InvisibleRoutePuzzle : MonoBehaviour
{
    [Header("Invisible Platforms")]
    public SpriteRenderer[] invisiblePlatforms;

    [Header("Reveal Settings")]
    public float revealDuration = 3f;

    [Header("Optional Door")]
    public Door doorToOpen;

    private bool isRevealing;
    private bool completed;

    private void Start()
    {
        HidePlatforms();
    }

    public void RevealPlatforms()
    {
        if (completed)
            return;

        if (isRevealing)
            return;

        StartCoroutine(RevealRoutine());
    }

    private IEnumerator RevealRoutine()
    {
        isRevealing = true;

        ShowPlatforms();

        yield return new WaitForSeconds(revealDuration);

        HidePlatforms();

        isRevealing = false;
    }

    private void ShowPlatforms()
    {
        foreach (SpriteRenderer platform in invisiblePlatforms)
        {
            if (platform != null)
                platform.enabled = true;
        }
    }

    private void HidePlatforms()
    {
        foreach (SpriteRenderer platform in invisiblePlatforms)
        {
            if (platform != null)
                platform.enabled = false;
        }
    }

    public void CompletePuzzle()
    {
        if (completed)
            return;

        completed = true;

        ShowPlatforms();

        if (doorToOpen != null)
            doorToOpen.ForceOpen();
    }
}