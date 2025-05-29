using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<Vector3> OnEnemyHit;
    public static event Action OnPlayerHit;
    public static event Action<EnemyController> OnEnemyDeath;
    public static event Action OnEnemySpawn;

    public static void TriggerEnemyHitEvent(Vector3 pos)
    {
        OnEnemyHit?.Invoke(pos);
    }

    public static void TriggerPlayerHitEvent()
    {
        OnPlayerHit?.Invoke();
    }

    public static void TriggerEnemyDeathEvent(EnemyController enemy)
    {
        OnEnemyDeath?.Invoke(enemy);
    }

    public static void TriggerEnemySpawn()
    {
        OnEnemySpawn?.Invoke();
    }
}
