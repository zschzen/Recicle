using DG.Tweening;
using Modules.BucketUpdate.Script;
using Modules.Collectable;
using Modules.Enemy;
using Modules.Wave;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.GameManager
{
    public class GameManager : MonoBehaviour, IBatchFixedUpdate
    {
        [SerializeField] private WaveManager m_waveManager;
        [SerializeField] private CollectableFactory m_collectableFactory;
        [SerializeField] private EnemyFactory m_enemyFactory;

        [SerializeField] private Dialogue.Dialogue m_introDialogue, m_winDialogue;
        [SerializeField] private float m_introDialogueDelay = .5f;

        private bool m_isGameStarted;

        // Unity methods ---------------------------------------------------------

        private void OnEnable()
        {
            UpdateManager.Instance.RegisterFixedUpdateSliced(this, 0);
        }

        private void OnDisable()
        {
            UpdateManager.Instance.DeregisterFixedUpdate(this);
        }

        private void Start()
        {
            DOVirtual.DelayedCall(m_introDialogueDelay, () => m_introDialogue.Show(true));

            m_introDialogue.OnDialogueEnd.AddListener(StartGame);
        }

        public void BatchFixedUpdate()
        {
            if (!m_isGameStarted) return;

            if (!m_waveManager.bIsWaveFinished) return;
            if (!m_waveManager.bEveryOneIsDead) return;

            m_isGameStarted = false;
            m_winDialogue.Show(true);
        }

        // Private methods ------------------------------------------------------
        
        private void StartGame()
        {
            // Spawn random collectable
            var amount = Random.Range(10, 15);

            for (var i = 0; i < amount; i++)
            {
                // set a new seed
                Random.InitState((int)Time.time + i);

                var collectable = m_collectableFactory.CreateRandomTypeCollectable();
                collectable.transform.position = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            }

            // Start wave
            m_waveManager.StartWave();

            m_isGameStarted = true;
        }
    }
}