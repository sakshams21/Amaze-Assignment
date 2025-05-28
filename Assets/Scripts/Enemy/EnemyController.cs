using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private BoxCollider[] Colliders;
    [SerializeField] private Renderer This_MeshRenderer;


    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private MaterialPropertyBlock _mpb;

    private int DissolvePropertyID = Shader.PropertyToID("_Dissolve");


    private int _animIDHit;
    private int _animIDAttack;
    private int _animIDSpeed;
    private int _animIDDie;

    private float health = 1;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
    }

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        AssignAnimId();
    }

    public void Setup()
    {
        foreach (var item in Colliders)
        {
            item.enabled = true;
        }

        This_MeshRenderer.SetPropertyBlock(_mpb);
        _mpb.SetFloat(DissolvePropertyID, 1);
        This_MeshRenderer.SetPropertyBlock(_mpb);
    }


    private void AssignAnimId()
    {
        _animIDHit = Animator.StringToHash("Hit");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDDie = Animator.StringToHash("Die");
    }

    public void Hit(AttacType type)
    {
        health -= 0.1f;
       

        if (type == AttacType.Primary)
        {
            PrimaryAttackHit();
        }
        else if (type == AttacType.Secondary)
        {
            SecondaryAttackHit();
        }

        if (health >= 0.5)
        {
            _animator.SetTrigger(_animIDHit);
        }
        else if (health < 0.5)
        {
            _animator.SetTrigger(_animIDDie);
        }
    }

    public void TriggerDissolve()
    {
        StartCoroutine(AfterDeath_Coro());
    }

    IEnumerator AfterDeath_Coro()
    {
        foreach (var item in Colliders)
        {
            item.enabled = false;
        }

        
        float duration = 1f;
        float startValue = 0;
        float targetValue = 1f;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

            _mpb.SetFloat(DissolvePropertyID, currentValue);
            This_MeshRenderer.SetPropertyBlock(_mpb);

            yield return null;
        }

        
        _mpb.SetFloat(DissolvePropertyID, targetValue);
        This_MeshRenderer.SetPropertyBlock(_mpb);

        //backtopool
    }

    private void PrimaryAttackHit()
    {
        transform.DOScale(Vector3.one * health, 0.2f).SetEase(Ease.InBounce);
    }

    private void SecondaryAttackHit()
    {
        //spawn red spheres which
    }
}
