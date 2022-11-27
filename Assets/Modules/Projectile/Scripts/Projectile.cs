using DG.Tweening;
using Enums;
using Modules.Factory;
using UnityEngine;

namespace Modules.Projectile
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class Projectile : FactoryBehaviour
    {
        [field: SerializeField] public DiscardTypes Type { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public int Damage { get; private set; } = 1;

        [SerializeField] private Rigidbody m_rigidbody;

        // Public methods -----------------------------------------------------------------------------------------------

        public void SetType(DiscardTypes type)
        {
            if (Type != DiscardTypes.None) return;

            Type = type;
        }

        public void Launch(Vector3 direction) => m_rigidbody.velocity = direction * Speed;

        // Unity Methods ----------------------------------------------------------------------

        private void OnEnable()
        {
            // Animate the projectile
            transform.localScale = Vector3.zero;
            _ = transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _ = DOTween.Kill(this);

            // Reset type
            Type = DiscardTypes.None;
        }

        private void OnBecameInvisible() => gameObject.SetActive(false);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Enemy.Enemy enemy)) return;
            if (enemy.IsDead) return;

            enemy.TakeDamage(Damage, Type);

            // Animate the projectile
            _ = transform.DOScale(0f, 0.1f)
                .OnComplete(() => gameObject.SetActive(false))
                .SetEase(Ease.InBack);

            // TODO: Add particle effect
        }
    }
}