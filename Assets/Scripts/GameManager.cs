using DG.Tweening;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
namespace Amaze
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { private set; get; }

        [Header("Enemy Related")]
        [SerializeField] private GameObject EnemyHitEffect;
        [SerializeField] private RectTransform EnemySpawnMessage;
        [SerializeField] private GameObject BloodSplatterEffect;
        [SerializeField] private EnemyPoolManager Ref_EnemyPoolManager;

        [Space(5f)]
        [SerializeField] private Canvas canvas;
        [SerializeField] private BoxCollider WeaponCollider;

        [SerializeField] private CinemachineImpulseSource impulseSource;

        public int TotalEnemiesAlive { get; private set; }

        public AttacType CurrentAttack { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            Instance = this;

            TotalEnemiesAlive = 1;
        }


        private void Start()
        {
            EventManager.OnEnemyHit += AfterHit;
            EventManager.OnEnemyDeath += ChangeEnemyValue;
        }



        private void OnDestroy()
        {
            EventManager.OnEnemyHit -= AfterHit;
            EventManager.OnEnemyDeath -= ChangeEnemyValue;
        }

        private void AfterHit(Vector3 pos)
        {
            Vector3 shakeStrength = (CurrentAttack == AttacType.Primary) ? Vector3.up * 0.1f : Vector3.one * 0.1f;
            impulseSource.GenerateImpulse(shakeStrength);
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

        public void SpawnEnemy(Vector3 position)
        {
            //check total number of enemies
            if (TotalEnemiesAlive >= 5) return;

            EnemySpawnMessage.gameObject.SetActive(true);
            EnemySpawnMessage.DOPunchScale(Vector3.one, 1.2f, 2, 0).OnComplete(() =>
            {
                EnemySpawnMessage.gameObject.SetActive(false);
            });

            StartCoroutine(SpawnEnemy_Coro(position));
        }

        IEnumerator SpawnEnemy_Coro(Vector3 position)
        {
            EnemyController enemy = Ref_EnemyPoolManager.GetFromPool();
            //spawn enemy facing the player
            BloodSplatterEffect.transform.localScale = new Vector3(1f, 0.01f, 1f);
            BloodSplatterEffect.SetActive(true);
            BloodSplatterEffect.transform.position = position;

            yield return new WaitForSeconds(1f);
            BloodSplatterEffect.transform.DOScale(new Vector3(0.1f, 0.01f, 0.1f), 1f).OnComplete(() =>
            {
                BloodSplatterEffect.SetActive(false);
                enemy.gameObject.SetActive(true);
                enemy.transform.position = position;
                enemy.Setup(transform.position);
                EventManager.TriggerEnemySpawn();
                TotalEnemiesAlive++;
            });
        }


        private void ChangeEnemyValue(EnemyController controller)
        {
            TotalEnemiesAlive = Mathf.Clamp(--TotalEnemiesAlive, 0, 5);
        }




    }

    public enum AttacType
    {
        None, Primary, Secondary
    }
}