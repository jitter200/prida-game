using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private string[] currentLines;
    private int currentLineIndex;
    private string currentNpcName;
    private Sprite currentPortrait;
    private bool isOpen;

    private void Awake()
    {
        Instance = this;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void StartDialogue(string npcName, Sprite npcPortrait, string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentNpcName = npcName;
        currentPortrait = npcPortrait;
        currentLines = lines;
        currentLineIndex = 0;

        isOpen = true;
        dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (nameText != null)
        {
            nameText.text = currentNpcName;
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = currentPortrait;
        }

        if (dialogueText != null)
        {
            dialogueText.text = currentLines[currentLineIndex];
        }
    }

    public void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            CloseDialogue();
            return;
        }

        ShowCurrentLine();
    }

    public void CloseDialogue()
    {
        isOpen = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}