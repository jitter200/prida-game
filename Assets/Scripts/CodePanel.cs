using UnityEngine;

public class CodePanel : MonoBehaviour
{
    [Header("Code Settings")]
    public string correctCode = "1234";

    [Header("References")]
    public CodeDoor codeDoor;

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (CodeDoorUI.Instance != null)
            {
                CodeDoorUI.Instance.OpenPanel(this);
            }
        }
    }

    public void CheckCode(string enteredCode)
    {
        if (enteredCode == correctCode)
        {
            codeDoor.OpenDoor();

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Код верный");
            }
        }
        else
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Неверный код");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы ввести код");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}