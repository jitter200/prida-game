using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Points")]
    public Transform[] points;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool loop = false;
    public bool startAutomatically = true;
    public float waitTimeAtPoint = 0f;

    private Rigidbody2D rb;
    private int currentPointIndex = 0;
    private int direction = 1;
    private bool isMoving;
    private float waitTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isMoving = startAutomatically;
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;
        if (points == null || points.Length == 0) return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Transform targetPoint = points[currentPointIndex];

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            targetPoint.position,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        if (Vector2.Distance(newPosition, targetPoint.position) <= 0.02f)
        {
            GoToNextPoint();
        }
    }

    private void GoToNextPoint()
    {
        waitTimer = waitTimeAtPoint;

        if (loop)
        {
            currentPointIndex++;

            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
        }
        else
        {
            currentPointIndex += direction;

            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = points.Length - 2;
                direction = -1;
            }
            else if (currentPointIndex < 0)
            {
                currentPointIndex = 1;
                direction = 1;
            }
        }
    }

    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        collision.transform.SetParent(null);
    }
}