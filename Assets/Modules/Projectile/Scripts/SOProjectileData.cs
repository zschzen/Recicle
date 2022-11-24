using Enums;
using UnityEngine;

namespace Modules.Projectile
{
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "Gameplay/Projectile", order = 0)]
    public class SOProjectileData : ScriptableObject
    {
        [field: SerializeField] public DiscardTypes Type { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float LifeTime { get; private set; }
        
        public float SpeedPerFrame => Speed * Time.deltaTime;
    }
}