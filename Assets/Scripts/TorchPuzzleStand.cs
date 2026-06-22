using UnityEngine;

public class TorchPuzzleStand : MonoBehaviour
{
    public TorchMemoryPuzzle puzzle;

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (puzzle != null)
            {
                puzzle.StartPuzzle();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы начать испытание");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}