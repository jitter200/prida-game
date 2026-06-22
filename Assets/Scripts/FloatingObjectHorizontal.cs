using UnityEngine;

public class FloatingObjectHorizontal : MonoBehaviour
{
    [Header("Horizontal Floating Settings")]
    [SerializeField] private float amplitude = 0.2f; // насколько далеко двигается влево-вправо
    [SerializeField] private float speed = 0.6f;     // скорость движения
    [SerializeField] private float phaseOffset = 0f; // сдвиг, чтобы объекты двигались не одинаково

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float xOffset = Mathf.Sin((Time.unscaledTime + phaseOffset) * speed) * amplitude;

        transform.localPosition = new Vector3(
            startPosition.x + xOffset,
            startPosition.y,
            startPosition.z
        );
    }
}