using UnityEngine;

public class RevealRouteButton : MonoBehaviour
{
    public InvisibleRoutePuzzle puzzle;

    private bool playerNear;

    private void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            if (puzzle != null)
                puzzle.RevealPlatforms();
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