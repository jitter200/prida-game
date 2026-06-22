using UnityEngine;

public class CodeStatue : MonoBehaviour
{
    [Header("Code Reward")]
    public int codeDigitIndex = 0;
    public char codeDigit = '9';
    public string rewardHint = "9***";

    private bool playerInRange = false;
    private bool rewardGiven = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            GiveCodeDigit();
        }
    }

    private void GiveCodeDigit()
    {
        if (rewardGiven)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Подсказка уже получена");
            }

            return;
        }

        rewardGiven = true;

        if (CodeProgress.Instance != null)
        {
            CodeProgress.Instance.SetKnownDigit(codeDigitIndex, codeDigit);
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Получена цифра кода: " + rewardHint);
        }

        Debug.Log("Statue gave code hint: " + rewardHint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E, чтобы осмотреть статую");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}