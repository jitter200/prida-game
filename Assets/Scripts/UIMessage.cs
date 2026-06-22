using TMPro;
using UnityEngine;

public class UIMessage : MonoBehaviour
{
    public static UIMessage Instance;

    public TextMeshProUGUI messageText;
    public float showTime = 2f;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        Instance = this;

        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        if (messageText == null) return;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        currentCoroutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private System.Collections.IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(showTime);

        messageText.gameObject.SetActive(false);
    }
}