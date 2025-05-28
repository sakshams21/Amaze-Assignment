using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { private set; get; }


    [SerializeField] private GameObject EnemyHitEffect;
    [SerializeField] private BoxCollider WeaponCollider;

    public AttacType CurrentAttack { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }


    private void Start()
    {
        EventManager.OnEnemyHit += AfterHit;
       
    }


    private void OnDestroy()
    {
        EventManager.OnEnemyHit -= AfterHit;
    }

    private void AfterHit(Vector3 pos)
    {
        EnemyHitEffect.transform.position = pos;
        EnemyHitEffect.SetActive(false);
        EnemyHitEffect.SetActive(true);
    }

    public void ChangeWeaponComponentStatus(bool status)
    {
        WeaponCollider.enabled = status;
    }

    public void SetCurrentAttack(AttacType current)
    {
        CurrentAttack = current;
    }
}

public enum AttacType
{
   None, Primary,Secondary
}
