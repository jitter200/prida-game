using UnityEngine;

public class MovingPlatformRide : MonoBehaviour
{
    [Header("Movement")]
    public Transform targetPoint;
    public float moveSpeed = 3f;
    public bool startOnPlayerStep = false;

    [Header("Interaction")]
    public KeyCode startKey = KeyCode.E;

    [Header("Flying Enemies")]
    public FlyingEnemyGroup flyingEnemyGroup;

    private Rigidbody2D rb;
    private bool playerOnPlatform = false;
    private bool isMoving = false;
    private bool hasArrived = false;
    private Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (hasArrived) return;

        if (startOnPlayerStep && playerOnPlatform && !isMoving)
        {
            StartMoving();
        }

        if (!startOnPlayerStep && playerOnPlatform && !isMoving && Input.GetKeyDown(startKey))
        {
            StartMoving();
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving || targetPoint == null) return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = targetPoint.position;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        float distance = Vector2.Distance(newPosition, targetPosition);

        if (distance <= 0.02f)
        {
            Arrive();
        }
    }

    private void StartMoving()
    {
        isMoving = true;
        if (flyingEnemyGroup != null && playerTransform != null)
        {
            flyingEnemyGroup.ActivateEnemies(playerTransform);
        }
        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Платформа движется");
        }


        Debug.Log("Moving platform started");
    }

    private void Arrive()
    {
        isMoving = false;
        hasArrived = true;

        rb.MovePosition(targetPoint.position);
        if (flyingEnemyGroup != null)
        {
            flyingEnemyGroup.StopEnemies();
        }
        if (playerTransform != null)
        {
            playerTransform.SetParent(null);
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Платформа прибыла");
        }

        Debug.Log("Moving platform arrived");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        playerOnPlatform = true;
        playerTransform = collision.transform;

        playerTransform.SetParent(transform);

        if (!startOnPlayerStep)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Нажми E, чтобы запустить платформу");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        playerOnPlatform = false;

        if (playerTransform != null)
        {
            playerTransform.SetParent(null);
            playerTransform = null;
        }
    }
}