using Enums;
using Modules.UIScreen;
using UnityEngine;

namespace Modules.Character
{
    [CreateAssetMenu(fileName = "Character Data", menuName = "Gameplay/Character Data", order = 0)]
    public class SOCharacterData : ScriptableObject
    {
        [field: Header("Attributes")]
        [field: SerializeField]
        public ValueNotify<int> Health { get; internal set; } = new(3);

        [field: SerializeField] public DiscardTypes Type { get; private set; }

        [field: SerializeField] public int MaxHealth { get; private set; } = 100;

        [field: SerializeField] public float Speed { get; private set; } = 5f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 10f;

        [field: SerializeField] public int Damage { get; private set; }

        [field: SerializeField] public float InteractionRange { get; private set; } = 1f;

        [field: Header("Visuals")]
        [field: SerializeField]
        public GameObject Body { get; private set; }
    }
}