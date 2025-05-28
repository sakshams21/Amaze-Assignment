using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<Vector3> OnEnemyHit;
    public static event Action OnPlayerHit;


    public static void TriggerEnemyHit(Vector3 pos)
    {
        OnEnemyHit?.Invoke(pos);
    }

    public static void TriggerPlayerHit()
    {
        OnPlayerHit?.Invoke();
    }
}
