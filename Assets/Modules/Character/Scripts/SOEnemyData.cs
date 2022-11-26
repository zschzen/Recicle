using UnityEngine;

namespace Modules.Character
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "Gameplay/Enemy Data", order = 1)]
    public class SOEnemyData : SOCharacterData
    {
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: SerializeField] public float DropTime { get; private set; }
        [field: SerializeField, Range(0, 1)] public float DropChance { get; private set; } = 0.5f;
    }
}