using UnityEngine;

public class PuzzleCheckButton : MonoBehaviour
{
    public SymbolPuzzle puzzle;

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (puzzle != null)
            {
                puzzle.CheckCombination();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы проверить комбинацию");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}