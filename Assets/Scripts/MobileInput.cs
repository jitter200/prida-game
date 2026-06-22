using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    [Header("Movement")]
    public float horizontal;

    [Header("Buttons")]
    public bool jumpPressed;
    public bool attackPressed;
    public bool dashPressed;
    public bool parryPressed;
    public bool ultimatePressed;
    public bool interactPressed;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        jumpPressed = false;
        attackPressed = false;
        dashPressed = false;
        parryPressed = false;
        ultimatePressed = false;
        interactPressed = false;
    }

    public void SetHorizontal(float value)
    {
        horizontal = value;
    }

    public void StopHorizontal()
    {
        horizontal = 0f;
    }

    public void PressJump()
    {
        jumpPressed = true;
    }

    public void PressAttack()
    {
        attackPressed = true;
    }

    public void PressDash()
    {
        dashPressed = true;
    }

    public void PressParry()
    {
        parryPressed = true;
    }

    public void PressUltimate()
    {
        ultimatePressed = true;
    }

    public void PressInteract()
    {
        interactPressed = true;
    }
}