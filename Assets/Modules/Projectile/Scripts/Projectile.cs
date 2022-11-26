using System;
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
        [field: SerializeField] public float LifeTime { get; private set; }

        [SerializeField] private Rigidbody m_rigidbody;
        [SerializeField] private Renderer m_renderer;

        // Public methods -----------------------------------------------------------------------------------------------

        public void Launch(Vector3 direction, DiscardTypes type)
        {
            Type = type;
            m_rigidbody.velocity = direction * Speed;
        }

        // Unity Methods ----------------------------------------------------------------------

        private void OnEnable()
        {
            // Animate the projectile
            transform.localScale = Vector3.zero;
            _ = transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
        }

        private void OnDisable()
        {
            _ = DOTween.Kill(this);
        }

        private void OnBecameInvisible() => gameObject.SetActive(false);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            if (!other.TryGetComponent(out Enemy.Enemy enemy)) return;

            enemy.TakeDamage(Damage, Type);

            // Animate the projectile
            _ = transform.DOScale(0f, 0.1f)
                .OnComplete(() => gameObject.SetActive(false))
                .SetEase(Ease.InBack);

            // TODO: Add particle effect
        }

        // Private Methods ---------------------------------------------------------------------

        private Color GetColorByType(DiscardTypes type)
        {
            return Color.white;
        }
    }
}