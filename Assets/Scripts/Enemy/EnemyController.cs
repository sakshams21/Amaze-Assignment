using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private BoxCollider[] Colliders;
    [SerializeField] private Renderer This_MeshRenderer;
    [SerializeField] private Transform CurrentHealth;


    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private MaterialPropertyBlock _mpb;

    private int DissolvePropertyID = Shader.PropertyToID("_Dissolve");


    private int _animIDHit;
    private int _animIDAttack;
    private int _animIDSpeed;
    private int _animIDDie;

    private float health = 1;
    private float currentScale = 1;

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

    public void Setup(Vector3 pos)
    {
        foreach (var item in Colliders)
        {
            item.enabled = true;
        }

        This_MeshRenderer.SetPropertyBlock(_mpb);
        _mpb.SetFloat(DissolvePropertyID, 0);
        This_MeshRenderer.SetPropertyBlock(_mpb);
        CurrentHealth.parent.gameObject.SetActive(true);

        CurrentHealth.localScale = new Vector3(1, 1.1f, 1.1f);

        Vector3 direction = pos - transform.position;
        direction.x = 0;
        direction.z = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }

        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, 1f);
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
        health -= 0.2f;
        
        CurrentHealth.DOScaleX(health, 0.2f).SetEase(Ease.Linear);

        if (type == AttacType.Primary)
        {
            PrimaryAttackHit();
        }
        else if (type == AttacType.Secondary)
        {
            SecondaryAttackHit();
        }

        if (health >= 0.1)
        {
            _animator.SetTrigger(_animIDHit);
        }
        else
        {
            _animator.SetTrigger(_animIDDie);
            CurrentHealth.parent.gameObject.SetActive(false);
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
        EventManager.TriggerEnemyDeathEvent(this);
    }

    private void PrimaryAttackHit()
    {
        currentScale -= 0.1f;
        transform.DOScale(Vector3.one * Mathf.Clamp(currentScale, 0.5f, 1f), 0.2f).SetEase(Ease.InBounce);
    }

    private void SecondaryAttackHit()
    {
        Vector3 randomOffset = Random.onUnitSphere * 5f;

        if (Vector3.Dot(randomOffset, transform.right) > 0)
        {
            randomOffset = -randomOffset;
        }

        Vector3 pos = transform.position + randomOffset;
        pos.y = 0;

        GameManager.Instance.SpawnEnemy(pos);
    }
}
