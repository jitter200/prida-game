using UnityEngine;

public class PendulumTrap : MonoBehaviour
{
    [Header("Swing")]
    public float swingAngle = 45f;
    public float swingSpeed = 2f;

    private Quaternion startRotation;

    private void Start()
    {
        startRotation = transform.rotation;
    }

    private void Update()
    {
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.rotation = startRotation * Quaternion.Euler(0f, 0f, angle);
    }
}