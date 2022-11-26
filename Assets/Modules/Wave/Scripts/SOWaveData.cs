using UnityEngine;

namespace Modules.UIScreen
{
    [CreateAssetMenu(fileName = "Wave Data", menuName = "Wave/Data", order = 0)]
    public class SOWaveData : ScriptableObject
    {
        [field: SerializeField] public int WaveCount { get; private set; }
        [field: SerializeField] public int EnemyCount { get; private set; }

        [field: SerializeField] public float SpawnDelay { get; private set; }
        [field: SerializeField] public uint WaveDelay { get; private set; }
    }
}