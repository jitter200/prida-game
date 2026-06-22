using UnityEngine;

public class SymbolPuzzle : MonoBehaviour
{
    [Header("Slots")]
    public SymbolSlot slot1;
    public SymbolSlot slot2;
    public SymbolSlot slot3;

    [Header("Correct Combination")]
    public PuzzleSymbol correctSlot1 = PuzzleSymbol.Star;
    public PuzzleSymbol correctSlot2 = PuzzleSymbol.Water;
    public PuzzleSymbol correctSlot3 = PuzzleSymbol.Arrow;

    [Header("Reward")]
    public bool rewardGiven = false;
    public int codeDigitIndex = 1;
    public char codeDigit = '5';
    public string rewardHint = "*5**";

    public void CheckCombination()
    {
        if (rewardGiven)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Головоломка уже решена");
            }

            return;
        }

        bool isCorrect =
            slot1.GetSymbol() == correctSlot1 &&
            slot2.GetSymbol() == correctSlot2 &&
            slot3.GetSymbol() == correctSlot3;

        if (isCorrect)
        {
            SolvePuzzle();
        }
        else
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Неверная комбинация");
            }

            Debug.Log("Wrong puzzle combination");
        }
    }

    private void SolvePuzzle()
    {
        rewardGiven = true;

        if (CodeProgress.Instance != null)
        {
            CodeProgress.Instance.SetKnownDigit(codeDigitIndex, codeDigit);
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Получена цифра кода: " + rewardHint);
        }

        Debug.Log("Symbol puzzle solved");
    }
}