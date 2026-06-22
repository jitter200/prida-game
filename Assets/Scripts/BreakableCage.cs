using UnityEngine;

public class BreakableCage : MonoBehaviour
{
    [Header("Settings")]
    public bool destroyOnHit = true;

    public void Break()
    {
        Debug.Log("Cage broken");

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}