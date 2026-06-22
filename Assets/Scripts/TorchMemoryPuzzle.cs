using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchMemoryPuzzle : MonoBehaviour
{
    [Header("Torches")]
    public PuzzleTorch torch1;
    public PuzzleTorch torch2;
    public PuzzleTorch torch3;

    [Header("Settings")]
    public int totalSteps = 10;
    public float flashTime = 0.5f;
    public float timeBetweenFlashes = 0.25f;
    public float timeBeforePlayerInput = 0.5f;

    [Header("Reward")]
    public bool givesCodeDigit = true;
    public int codeDigitIndex = 3;
    public char codeDigit = '7';
    public string rewardHint = "***7";

    private List<int> sequence = new List<int>();

    private int currentRound = 0;
    private int playerInputIndex = 0;

    private bool puzzleActive = false;
    private bool showingSequence = false;
    private bool waitingForInput = false;
    private bool solved = false;

    private void Start()
    {
        SetupTorches();
    }

    private void SetupTorches()
    {
        if (torch1 != null)
        {
            torch1.torchID = 1;
            torch1.puzzle = this;
        }

        if (torch2 != null)
        {
            torch2.torchID = 2;
            torch2.puzzle = this;
        }

        if (torch3 != null)
        {
            torch3.torchID = 3;
            torch3.puzzle = this;
        }
    }

    public void StartPuzzle()
    {
        if (solved)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Испытание уже пройдено");
            }

            return;
        }

        GenerateSequence();

        puzzleActive = true;
        currentRound = 1;
        playerInputIndex = 0;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Запомни порядок факелов");
        }

        StartCoroutine(ShowSequence());
    }

    private void GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < totalSteps; i++)
        {
            int randomTorch = Random.Range(1, 4); // 1, 2 или 3
            sequence.Add(randomTorch);
        }

        Debug.Log("Torch sequence: " + string.Join(",", sequence));
    }

    private IEnumerator ShowSequence()
    {
        showingSequence = true;
        waitingForInput = false;
        playerInputIndex = 0;

        yield return new WaitForSeconds(timeBeforePlayerInput);

        for (int i = 0; i < currentRound; i++)
        {
            PuzzleTorch torch = GetTorchByID(sequence[i]);

            if (torch != null)
            {
                torch.Flash(flashTime);
            }

            yield return new WaitForSeconds(flashTime + timeBetweenFlashes);
        }

        showingSequence = false;
        waitingForInput = true;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Повтори комбинацию");
        }
    }

    public void PlayerHitTorch(int torchID)
    {
        if (!puzzleActive) return;
        if (showingSequence) return;
        if (!waitingForInput) return;
        if (solved) return;

        PuzzleTorch torch = GetTorchByID(torchID);

        if (torch != null)
        {
            torch.Flash(0.2f);
        }

        int correctTorch = sequence[playerInputIndex];

        if (torchID != correctTorch)
        {
            FailPuzzle();
            return;
        }

        playerInputIndex++;

        if (playerInputIndex >= currentRound)
        {
            CompleteRound();
        }
    }

    private void CompleteRound()
    {
        waitingForInput = false;

        if (currentRound >= totalSteps)
        {
            SolvePuzzle();
            return;
        }

        currentRound++;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Верно. Следующий шаг: " + currentRound);
        }

        StartCoroutine(ShowSequence());
    }

    private void FailPuzzle()
    {
        puzzleActive = false;
        waitingForInput = false;
        showingSequence = false;

        TurnOffAllTorches();

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Ошибка. Испытание начнётся заново");
        }

        Debug.Log("Torch puzzle failed");
    }

    private void SolvePuzzle()
    {
        solved = true;
        puzzleActive = false;
        waitingForInput = false;
        showingSequence = false;

        TurnOffAllTorches();

        if (givesCodeDigit && CodeProgress.Instance != null)
        {
            CodeProgress.Instance.SetKnownDigit(codeDigitIndex, codeDigit);
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Испытание пройдено. Получена цифра кода: " + rewardHint);
        }

        Debug.Log("Torch puzzle solved");
    }

    private void TurnOffAllTorches()
    {
        if (torch1 != null) torch1.SetLit(false);
        if (torch2 != null) torch2.SetLit(false);
        if (torch3 != null) torch3.SetLit(false);
    }

    private PuzzleTorch GetTorchByID(int id)
    {
        if (id == 1) return torch1;
        if (id == 2) return torch2;
        if (id == 3) return torch3;

        return null;
    }
}