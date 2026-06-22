using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerRespawn playerRespawn = collision.GetComponent<PlayerRespawn>();

        if (playerRespawn != null)
        {
            playerRespawn.SetCheckpoint(respawnPoint.position);
            Debug.Log("Checkpoint activated");
        }
    }
}