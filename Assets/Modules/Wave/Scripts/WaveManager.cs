using System;
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

        private int m_currentWaveIndex;

        private Enemy.Enemy m_enemyRef;
        private IObjectPool<Enemy.Enemy> m_enemyPool;

        // Unity Methods --------------------------------------------------------

        private void Awake()
        {
            // Get enemy prefab from addressables
            m_enemyRef = Addressables.LoadAssetAsync<Enemy.Enemy>("Enemy").WaitForCompletion();

            // Create pool
            m_enemyPool = new ObjectPool<Enemy.Enemy>(
                CreatePooleableProjectile, OnTakeFromPool,
                OnReturnedToPool, OnDestroyPoolObject,
                false, 10, 10
            );
        }

        private void Start()
        {
            m_currentWaveIndex = 0;
        }

        // Private Methods ------------------------------------------------------

        private Enemy.Enemy CreatePooleableProjectile()
        {
            // Clone the enemy prefab
            var enemy = Instantiate(m_enemyRef);

            // Set the projectile release method
            enemy.OnRelease += () => m_enemyPool.Release(enemy);

            // Return the projectile
            return enemy;
        }

        private void OnReturnedToPool(Enemy.Enemy projectile)
        {
            // Set the projectile to inactive
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(Enemy.Enemy projectile)
        {
            // Destroy the projectile
            Destroy(projectile.gameObject);
        }

        private void OnTakeFromPool(Enemy.Enemy projectile)
        {
            // Set the projectile to active
            projectile.gameObject.SetActive(true);
        }

        private Transform GetRandomSpawnPoint() => m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];

        private void SpawnWave()
        {
            if (m_currentWaveIndex >= m_waveData.TotalWaveCount)
            {
                Debug.Log("No more waves");
                return;
            }

            for (int i = 0; i < m_waveData.EnemyCount; i++)
            {
                var spawnPoint = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];
                //Instantiate(m_waveData.EnemyPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }
}