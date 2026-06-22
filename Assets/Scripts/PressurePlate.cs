using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("Plate ID")]
    public int requiredBoxID = 1;

    [Header("State")]
    public bool isPressed = false;

    [Header("Puzzle")]
    public TwoBoxPuzzle puzzle;

    private int correctBoxesOnPlate = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CarryableBox box = collision.GetComponent<CarryableBox>();

        if (box == null) return;

        if (box.boxID != requiredBoxID)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Ёта коробка не подходит");
            }

            return;
        }

        correctBoxesOnPlate++;
        UpdateState();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CarryableBox box = collision.GetComponent<CarryableBox>();

        if (box == null) return;

        if (box.boxID != requiredBoxID) return;

        correctBoxesOnPlate--;

        if (correctBoxesOnPlate < 0)
        {
            correctBoxesOnPlate = 0;
        }

        UpdateState();
    }

    private void UpdateState()
    {
        bool newState = correctBoxesOnPlate > 0;

        if (isPressed == newState)
        {
            return;
        }

        isPressed = newState;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage(isPressed ? " нопка нажата" : " нопка отпущена");
        }

        if (puzzle != null)
        {
            puzzle.CheckPuzzle();
        }
    }
}