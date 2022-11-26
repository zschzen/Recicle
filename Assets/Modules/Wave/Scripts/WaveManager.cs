using System.Collections;
using Enums;
using Modules.UIScreen;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Modules.Wave
{
    public class WaveManager : MonoBehaviour
    {
        // Fields ----------------------------------------------------------------
        [SerializeField] private SOWaveData m_waveData;
        [SerializeField] private Transform[] m_spawnPoints;

        private ValueNotify<int> m_currentWaveIndex = new(0);

        private Enemy.Enemy m_enemyRef;
        private IObjectPool<Enemy.Enemy> m_enemyPool;

        private Coroutine m_waveCoroutine;

        // Public Methods -------------------------------------------------------

        public void StartWave()
        {
            if (m_waveCoroutine != null) StopCoroutine(m_waveCoroutine);

            m_currentWaveIndex.value = 0;
            m_waveCoroutine = StartCoroutine(WaveRoutine());
        }

        // Unity Methods --------------------------------------------------------

        private void Awake()
        {
            // Get enemy prefab from addressables
            m_enemyRef = Addressables.LoadAssetAsync<GameObject>("Enemy")
                .WaitForCompletion()
                .GetComponent<Enemy.Enemy>();

            // Create pool
            m_enemyPool = new ObjectPool<Enemy.Enemy>(
                CreatePooleableEnemy, OnTakeFromPool,
                OnReturnedToPool, OnDestroyPoolObject,
                false, 30, 30
            );
        }

        private void Start()
        {
            m_currentWaveIndex.value = 0;

            // Start wave
            StartWave();
        }

        // Private Methods ------------------------------------------------------

        /// <summary>
        /// Get random spawn point position
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomSpawnPoint() => m_spawnPoints[Random.Range(0, m_spawnPoints.Length)].position;

        /// <summary>
        /// Get random value that its not None or Recycable or NonRecycable
        /// </summary>
        /// <returns>Random <see cref="DiscardTypes"/></returns>
        private DiscardTypes GetRandomDiscardType()
        {
            var randomValue = Random.Range(1, 3);
            return (DiscardTypes)randomValue;
        }

        /// <summary>
        /// Keep spawning enemies between delays
        /// TODO: A nice addition would be to display the remaining enemies in the wave or even the remaining time to the next wave
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaveRoutine()
        {
            var spawnDelay = new WaitForSeconds(m_waveData.SpawnDelay);
            var waveDelay = new WaitForSeconds(m_waveData.WaveDelay);

            do
            {
                // For each enemy count in single wave
                for (int i = 0; i < m_waveData.EnemyCount; i++)
                {
                    // Do spawn enemy
                    SpawnEnemy();

                    // Wait between spawns
                    yield return spawnDelay;
                }

                // Wait between waves
                yield return waveDelay;

                // Increase wave index while it's less than wave count
            } while (++m_currentWaveIndex.value < m_waveData.WaveCount);

            SpawnBoss();
        }

        // Spawn enemy at random spawn point
        private void SpawnEnemy()
        {
            var enemy = m_enemyPool.Get();
            enemy.SetType(GetRandomDiscardType());
            enemy.transform.position = GetRandomSpawnPoint();
        }

        /// <summary>
        /// Set enemy's boss
        /// </summary>
        private void SpawnBoss()
        {
            var enemy = m_enemyPool.Get().transform;
            enemy.position = GetRandomSpawnPoint();
            enemy.localScale = Vector3.one * 2;
        }

        private Enemy.Enemy CreatePooleableEnemy()
        {
            // Clone the enemy prefab
            var enemy = Instantiate(m_enemyRef);

            // Set the projectile release method
            enemy.OnRelease += () => m_enemyPool.Release(enemy);

            // Return the projectile
            return enemy;
        }

        private void OnReturnedToPool(Enemy.Enemy enemy)
        {
            // Set the projectile to inactive
            enemy.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(Enemy.Enemy enemy)
        {
            // Destroy the projectile
            Destroy(enemy.gameObject);
        }

        private void OnTakeFromPool(Enemy.Enemy enemy)
        {
            // Set the projectile to active
            enemy.gameObject.SetActive(true);
        }
    }
}