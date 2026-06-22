using UnityEngine;

public class NineDigitPanel : MonoBehaviour
{
    [Header("Code Settings")]
    public string correctCode = "123456789";

    [Header("References")]
    public NineDigitDoor door;

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("OPEN PANEL FROM INTERACT ZONE");

            if (NineDigitDoorUI.Instance != null)
            {
                NineDigitDoorUI.Instance.OpenPanel(this);
            }
            else
            {
                Debug.LogError("NineDigitDoorUI.Instance is null");
            }
        }
    }

    public void CheckCode(string enteredCode)
    {
        Debug.Log("Entered code: " + enteredCode);
        Debug.Log("Correct code: " + correctCode);

        if (enteredCode == correctCode)
        {
            Debug.Log("CORRECT CODE");

            if (door != null)
            {
                door.OpenDoor();
            }
            else
            {
                Debug.LogError("NineDigitPanel: Door is not assigned");
            }

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Комбинация верная");
            }
        }
        else
        {
            Debug.Log("WRONG CODE");

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Неправильная комбинация чисел");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E для ввода кода");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}