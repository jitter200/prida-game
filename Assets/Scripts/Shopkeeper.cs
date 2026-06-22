using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    private bool playerInRange = false;
    private PlayerHealth playerHealth;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (ShopUI.Instance != null)
            {
                ShopUI.Instance.OpenShop(playerHealth);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;
        playerHealth = collision.GetComponent<PlayerHealth>();

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы открыть магазин");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        playerHealth = null;
    }
}