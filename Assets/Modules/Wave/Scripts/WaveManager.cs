using UnityEngine;

namespace Modules.Wave
{
    public class WaveManager : MonoBehaviour
    {
        // Fields ----------------------------------------------------------------
        [SerializeField] private SOWaveData m_waveData;
        [SerializeField] private Transform[] m_spawnPoints;
        
        private int m_currentWaveIndex;

        // Unity Methods --------------------------------------------------------
        
        private void Start()
        {
            m_currentWaveIndex = 0;
            SpawnWave();
        }
        
        // Private Methods ------------------------------------------------------
        
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