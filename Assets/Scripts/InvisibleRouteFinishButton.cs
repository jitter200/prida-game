using UnityEngine;

public class InvisibleRouteFinishButton : MonoBehaviour
{
    public InvisibleRoutePuzzle puzzle;

    private bool playerNear;
    private bool used;

    private void Update()
    {
        if (used)
            return;

        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            used = true;

            if (puzzle != null)
                puzzle.CompletePuzzle();

            transform.localScale = new Vector3(1f, 0.5f, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNear = false;
    }
}