using UnityEngine;

public class LaserMirror : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 1.5f;

    [Header("Rotation")]
    public float rotationStep = 45f;

    private Transform player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactDistance && Input.GetKeyDown(interactKey))
        {
            RotateMirror();
        }
    }

    private void RotateMirror()
    {
        transform.Rotate(0f, 0f, -rotationStep);

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Зеркало повернуто");
        }
    }

    public Vector2 Reflect(Vector2 incomingDirection)
    {
        Vector2 normal = transform.right;

        if (Vector2.Dot(incomingDirection, normal) > 0f)
        {
            normal = -normal;
        }

        return Vector2.Reflect(incomingDirection, normal).normalized;
    }
}