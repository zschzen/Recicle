using Modules.Wave;
using UnityEngine;

namespace Modules.Levels.Scripts
{
    [CreateAssetMenu(fileName = "Level Data", menuName = "Gameplay/Level", order = 0)]
    public class SOLevel : ScriptableObject
    {
        [field: SerializeField] public SOWaveData WaveData { get; private set; }
        [field: SerializeField] public int Level { get; private set; }
        [field: SerializeField] public int Waves { get; private set; }
    }
}