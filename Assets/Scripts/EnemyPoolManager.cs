using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Amaze
{
    public class EnemyPoolManager : MonoBehaviour
    {

        [SerializeField] private EnemyController EnemyPrefab;

        private Queue<EnemyController> _enemyPool = new Queue<EnemyController>();

        private int _initialPoolSize = 10;

        private async void Start()
        {
            EventManager.OnEnemyDeath += BackToPool;
            await InitializePoolAsync(_initialPoolSize);
        }

        private void OnDestroy()
        {
            EventManager.OnEnemyDeath -= BackToPool;
        }

        private async Task InitializePoolAsync(int count)
        {
            AsyncInstantiateOperation<EnemyController> handle = InstantiateAsync(EnemyPrefab, _initialPoolSize, transform);
            await handle;

            if (handle.isDone)
            {
                foreach (var item in handle.Result)
                {
                    item.gameObject.SetActive(false);
                    _enemyPool.Enqueue(item);
                }

            }
            else
            {
                Debug.LogError("Failed to instantiate enemy prefab.");
            }
        }


        public EnemyController GetFromPool()
        {
            if (_enemyPool.Count == 0)
            {
                EnemyController newEnemy = Instantiate(EnemyPrefab, transform);
                newEnemy.gameObject.SetActive(false);
                return newEnemy;
            }

            return _enemyPool.Dequeue();
        }


        public void BackToPool(EnemyController enemy)
        {
            enemy.gameObject.SetActive(false);
            _enemyPool.Enqueue(enemy);
        }

    }
}