using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Laser")]
    public Transform firePoint;
    public float maxDistance = 30f;
    public int maxReflections = 10;
    public LayerMask laserHitLayer;

    [Header("Line")]
    public float lineWidth = 0.05f;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    private void Update()
    {
        DrawLaser();
    }

    private void DrawLaser()
    {
        if (firePoint == null) return;

        List<Vector3> points = new List<Vector3>();

        Vector2 origin = firePoint.position;
        Vector2 direction = firePoint.right;

        points.Add(origin);

        for (int i = 0; i <= maxReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                direction,
                maxDistance,
                laserHitLayer
            );

            if (hit.collider == null)
            {
                points.Add(origin + direction * maxDistance);
                break;
            }

            points.Add(hit.point);

            LaserReceiver receiver = hit.collider.GetComponent<LaserReceiver>();
            if (receiver == null)
            {
                receiver = hit.collider.GetComponentInParent<LaserReceiver>();
            }

            if (receiver != null)
            {
                receiver.ReceiveLaser();
                break;
            }

            LaserMirror mirror = hit.collider.GetComponent<LaserMirror>();
            if (mirror == null)
            {
                mirror = hit.collider.GetComponentInParent<LaserMirror>();
            }

            if (mirror != null)
            {
                direction = mirror.Reflect(direction);
                origin = hit.point + direction * 0.05f;
                continue;
            }

            break;
        }

        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }
}