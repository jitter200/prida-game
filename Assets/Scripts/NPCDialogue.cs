using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName;
    public Sprite npcPortrait;

    [TextArea(2, 5)]
    public string[] dialogueLines;

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange) return;

        if (IsInteractPressed())
        {
            if (DialogueUI.Instance == null) return;

            if (!DialogueUI.Instance.IsOpen())
            {
                DialogueUI.Instance.StartDialogue(npcName, npcPortrait, dialogueLines);
            }
        }
    }

    private bool IsInteractPressed()
    {
        return Input.GetKeyDown(KeyCode.E) ||
               (MobileInput.Instance != null && MobileInput.Instance.interactPressed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Нажми E / кнопку взаимодействия, чтобы поговорить");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}