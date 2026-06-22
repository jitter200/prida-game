using UnityEngine;

public class SymbolSlot : MonoBehaviour
{
    [Header("Slot")]
    public PuzzleSymbol currentSymbol = PuzzleSymbol.Star;

    [Header("Sprites")]
    public Sprite[] symbolSprites;

    [Header("Visual")]
    public SpriteRenderer symbolRenderer;

    private bool playerInRange = false;
    private int currentIndex = 0;

    private void Start()
    {
        currentIndex = (int)currentSymbol;
        UpdateVisual();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            NextSymbol();
        }
    }

    private void NextSymbol()
    {
        currentIndex++;

        if (currentIndex >= symbolSprites.Length)
        {
            currentIndex = 0;
        }

        currentSymbol = (PuzzleSymbol)currentIndex;
        UpdateVisual();

        Debug.Log(gameObject.name + " symbol: " + currentSymbol);
    }

    private void UpdateVisual()
    {
        if (symbolRenderer != null && symbolSprites != null && symbolSprites.Length > 0)
        {
            symbolRenderer.sprite = symbolSprites[currentIndex];
        }
    }

    public PuzzleSymbol GetSymbol()
    {
        return currentSymbol;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы сменить символ");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}