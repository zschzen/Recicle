using System;
using System.Collections;
using Enums;
using Modules.UIScreen;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Modules.UIScreen
{
    public class WaveManager : MonoBehaviour
    {
        // Fields ----------------------------------------------------------------
        [SerializeField] private SOWaveData m_waveData;
        [SerializeField] private Transform[] m_spawnPoints;

        private ValueNotify<int> m_currentWaveIndex = new(0);
        private ValueNotify<uint> m_timeToNextWave = new(0);

        private Enemy.Enemy m_enemyRef;
        private IObjectPool<Enemy.Enemy> m_enemyPool;

        private Coroutine m_waveCoroutine;
        private Coroutine m_spawnCoroutine;
        private UIScreenWave m_waveScreen;

        // Public Methods -------------------------------------------------------

        public void StartWave()
        {
            if (m_waveCoroutine != null) StopCoroutine(m_waveCoroutine);

            m_currentWaveIndex.value = 0;

            m_timeToNextWave.value = m_waveData.WaveDelay;
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

            // Show wave screen
            LoadUI();
        }

        // Private Methods ------------------------------------------------------

        private void LoadUI()
        {
            // Addressables load wavehud
            var hudRef = Addressables.LoadAssetAsync<GameObject>("HUD/Wave")
                .WaitForCompletion()
                .GetComponent<UIScreenWave>();

            // Instantiate
            m_waveScreen = Instantiate(hudRef);

            // Register to events
            m_currentWaveIndex.OnChange += OnCurrentWaveIndexChanged;
            m_timeToNextWave.OnChange += OnTimeToNextWaveChanged;

            // Manually call event
            OnCurrentWaveIndexChanged();
            OnTimeToNextWaveChanged();
        }

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
            do
            {
                // kills m_spawnCoroutine
                if (m_spawnCoroutine != null) StopCoroutine(m_spawnCoroutine);
                m_spawnCoroutine = StartCoroutine(SpawnRoutine());

                // Wait the coroutine Countdown()
                yield return WaveCountdown();

                // Increase wave index while it's less than wave count
            } while (++m_currentWaveIndex.value < m_waveData.WaveCount);

            SpawnBoss();
        }

        private IEnumerator WaveCountdown()
        {
            float normalizedTime = 0;
            while (normalizedTime <= 1f)
            {
                m_timeToNextWave.value = (uint)(m_waveData.WaveDelay * (1 - normalizedTime));
                normalizedTime += Time.deltaTime / m_waveData.WaveDelay;
                yield return null;
            }
        }
        
        private IEnumerator SpawnRoutine()
        {
            var spawnDelay = new WaitForSeconds(m_waveData.SpawnDelay);

            // Spawn enemies
            for (int i = 0; i < m_waveData.EnemyCount; i++)
            {
                // Get random spawn point
                var spawnPoint = GetRandomSpawnPoint();

                // Get random discard type
                var discardType = GetRandomDiscardType();

                // Spawn enemy
                var enemy = m_enemyPool.Get();
                enemy.transform.position = spawnPoint;
                enemy.SetType(discardType);

                // Wait for spawn delay
                yield return spawnDelay;
            }
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

        private void OnTimeToNextWaveChanged() => m_waveScreen.SetTimer(m_timeToNextWave.value);

        private void OnCurrentWaveIndexChanged() => m_waveScreen.UpdateWaveText(m_currentWaveIndex.value + 1);
    }
}