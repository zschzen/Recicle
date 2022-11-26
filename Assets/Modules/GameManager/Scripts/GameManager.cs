using System;
using System.Collections;
using System.Reflection;
using DG.DOTweenEditor;
using DG.Tweening;
using Modules.Collectable;
using Modules.Dialogue;
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

        [SerializeField] private Dialogue.Dialogue m_introDialogue, m_gameoverDialogue, m_winDialogue;
        [SerializeField] private float m_introDialogueDelay = .5f;

        private void Start()
        {
            DOVirtual.DelayedCall(m_introDialogueDelay, () => m_introDialogue.Show(true));

            m_introDialogue.OnDialogueEnd.AddListener(StartGame);
        }

        // Start coroutine
        private void StartGame()
        {
            StartCoroutine(GameStartRoutine());
        }

        private IEnumerator GameStartRoutine()
        {
            // Spawn random collectable
            var amount = Random.Range(10, 20);

            for (var i = 0; i < amount; i++)
            {
                // set a new seed
                Random.InitState((int)Time.time + i);

                var collectable = m_collectableFactory.CreateRandomTypeCollectable();
                collectable.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            }
            
            yield return new WaitForSeconds(m_gameDelay);

            // Start wave
            m_waveManager.StartWave();
        }
    }
}