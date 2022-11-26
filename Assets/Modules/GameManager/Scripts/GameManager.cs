using System;
using System.Collections;
using Modules.Collectable;
using Modules.Wave;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float m_gameDelay = 3f;

        [SerializeField] private WaveManager m_waveManager;
        [SerializeField] private CollectableFactory m_collectableFactory;

        // Start coroutine
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(m_gameDelay);

            // Spawn random collectable
            var amount = Random.Range(5, 10);

            for (var i = 0; i < amount; i++)
            {
                var collectable = m_collectableFactory.CreateRandomCollectable();
                collectable.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            }
            
            yield return new WaitForSeconds(m_gameDelay);

            // Start wave
            m_waveManager.StartWave();
        }
    }
}