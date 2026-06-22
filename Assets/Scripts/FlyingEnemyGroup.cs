using UnityEngine;

public class FlyingEnemyGroup : MonoBehaviour
{
    [Header("Enemies")]
    public FlyingMeleeEnemy[] enemies;

    [Header("Settings")]
    public bool destroyEnemiesOnFinish = false;

    public void ActivateEnemies(Transform player)
    {
        gameObject.SetActive(true);

        foreach (FlyingMeleeEnemy enemy in enemies)
        {
            if (enemy == null) continue;

            enemy.gameObject.SetActive(true);
            enemy.SetTarget(player);
        }

        Debug.Log("Flying enemies activated");
    }

    public void StopEnemies()
    {
        foreach (FlyingMeleeEnemy enemy in enemies)
        {
            if (enemy == null) continue;

            if (destroyEnemiesOnFinish)
            {
                Destroy(enemy.gameObject);
            }
            else
            {
                enemy.SetTarget(null);
                enemy.gameObject.SetActive(false);
            }
        }

        Debug.Log("Flying enemies stopped");
    }
}