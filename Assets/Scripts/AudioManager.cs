using System;
using UnityEngine;

namespace Amaze
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip Hit_Primary_AudioClip;
        [SerializeField] private AudioClip Hit_Secondary_AudioClip;
        [SerializeField] private AudioClip EnemySpawn_AudioClip;
        [SerializeField] private AudioClip EnemyDie_AudioClip;

        private AudioSource _feedback_AudioSource;

        private void Start()
        {
            _feedback_AudioSource = GetComponent<AudioSource>();

            EventManager.OnEnemyHit += OnEnemyHitSound;
            EventManager.OnEnemySpawn += OnEnemySpawnSound;
            EventManager.OnEnemyDeath += OnEnemyDeathSound;
        }


        private void OnDestroy()
        {
            EventManager.OnEnemyHit -= OnEnemyHitSound;
            EventManager.OnEnemySpawn -= OnEnemySpawnSound;
            EventManager.OnEnemyDeath -= OnEnemyDeathSound;
        }

        private void OnEnemyDeathSound(EnemyController controller)
        {
            _feedback_AudioSource.PlayOneShot(EnemyDie_AudioClip);
        }

        private void OnEnemyHitSound(Vector3 vector)
        {
            _feedback_AudioSource.PlayOneShot((GameManager.Instance.CurrentAttack == AttacType.Primary) ? Hit_Primary_AudioClip : Hit_Secondary_AudioClip);
        }

        private void OnEnemySpawnSound()
        {
            _feedback_AudioSource.PlayOneShot(EnemySpawn_AudioClip);
        }

    }
}