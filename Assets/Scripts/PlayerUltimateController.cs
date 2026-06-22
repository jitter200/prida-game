using UnityEngine;

public class PlayerUltimateController : MonoBehaviour
{
    [Header("Input")]
    public KeyCode ultimateKey = KeyCode.Q;

    [Header("Ultimates")]
    public Ultimate_Rewind rewindUltimate;
    public Ultimate_Shockwave shockwaveUltimate;
    public Ultimate_Heal healUltimate;
    public Ultimate_LightningStrike lightningUltimate;

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        bool ultimateInput =
            Input.GetKeyDown(ultimateKey) ||
            (MobileInput.Instance != null && MobileInput.Instance.ultimatePressed);

        if (ultimateInput)
        {
            UseCurrentUltimate();
        }
    }

    private void UseCurrentUltimate()
    {
        if (PlayerLoadout.Instance == null)
        {
            Debug.LogWarning("PlayerLoadout not found");
            return;
        }

        string currentUltimate = PlayerLoadout.Instance.currentUltimate;

        Debug.Log("Current ult: " + currentUltimate);

        if (currentUltimate == "Rewind")
        {
            if (rewindUltimate != null)
                rewindUltimate.Use();
            else
                Debug.LogWarning("Rewind ultimate is not assigned");
        }
        else if (currentUltimate == "Shockwave")
        {
            if (shockwaveUltimate != null)
                shockwaveUltimate.Use();
            else
                Debug.LogWarning("Shockwave ultimate is not assigned");
        }
        else if (currentUltimate == "Heal")
        {
            if (healUltimate != null)
                healUltimate.Use();
            else
                Debug.LogWarning("Heal ultimate is not assigned");
        }
        else if (currentUltimate == "Lightning")
        {
            if (lightningUltimate != null)
                lightningUltimate.Use();
            else
                Debug.LogWarning("Lightning ultimate is not assigned");
        }
        else
        {
            Debug.Log("No ultimate equipped");
        }
    }
}