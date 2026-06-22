using System.Collections;
using UnityEngine;

public class PuzzleTorch : MonoBehaviour
{
    [Header("Torch ID")]
    public int torchID = 1;

    [Header("Sprites")]
    public Sprite unlitSprite;
    public Sprite litSprite;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    [Header("Puzzle")]
    public TorchMemoryPuzzle puzzle;

    private bool isLit = false;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        SetLit(false);
    }

    public void HitByPlayer()
    {
        if (puzzle != null)
        {
            puzzle.PlayerHitTorch(torchID);
        }
    }

    public void SetLit(bool value)
    {
        isLit = value;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isLit ? litSprite : unlitSprite;
        }
    }

    public void Flash(float duration)
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine(duration));
    }

    private IEnumerator FlashRoutine(float duration)
    {
        SetLit(true);

        yield return new WaitForSeconds(duration);

        SetLit(false);
        flashCoroutine = null;
    }
}