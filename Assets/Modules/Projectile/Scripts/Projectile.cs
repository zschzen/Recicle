using DG.Tweening;
using UnityEngine;

namespace Modules.Projectile
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [field: SerializeField] public SOProjectileData ProjectileData { get; private set; }

        [SerializeField] private Rigidbody m_rigidbody;

        // Unity Methods ----------------------------------------------------------------------

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
            OnLifeTimeEnd();
        }

        private void Start()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_rigidbody.velocity = transform.forward * ProjectileData.Speed;

            // Destroy the projectile after a certain amount of time
            DOVirtual.DelayedCall(ProjectileData.LifeTime, OnLifeTimeEnd).SetId(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;

            other.gameObject.GetComponent<Enemy.Enemy>().TakeDamage(ProjectileData.Damage, ProjectileData.Type);
            Destroy(gameObject);

            // TODO: Add particle effect
        }

        // Private Methods ---------------------------------------------------------------------
        private void OnLifeTimeEnd()
        {
            _ = DOTween.Kill(this);
            // TODO: Return to pool
        }
    }
}