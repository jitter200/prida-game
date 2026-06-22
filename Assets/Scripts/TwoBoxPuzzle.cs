using UnityEngine;

public class TwoBoxPuzzle : MonoBehaviour
{
    [Header("Plates")]
    public PressurePlate plate1;
    public PressurePlate plate2;

    [Header("Door")]
    public CodeDoor doorToOpen;

    private bool solved = false;

    public void CheckPuzzle()
    {
        if (solved) return;

        if (plate1 != null && plate2 != null && plate1.isPressed && plate2.isPressed)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        solved = true;

        if (doorToOpen != null)
        {
            doorToOpen.OpenDoor();
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Дверь открыта");
        }

        Debug.Log("Two box puzzle solved");
    }
}