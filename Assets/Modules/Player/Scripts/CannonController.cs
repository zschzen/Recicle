using System;
using DG.Tweening;
using Enums;
using Modules.Character;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Modules.Player
{
    public class CannonController : ABaseCharacter<SOCharacterData>
    {
        internal Func<bool> bHasAmmo;
        internal Func<int> GetAmmoCount;
        internal Func<DiscardTypes> GetAmmoType;

        // Fields ------------------------------------------

        private Projectile.Projectile m_projecRef;
        private IObjectPool<Projectile.Projectile> m_projectilePool;

        // Public Methods ----------------------------------------------------

        public override void Attack()
        {
            if (!bHasAmmo.Invoke()) return;
            if (DOTween.IsTweening(this)) return;

            // Take ammo
            var ammo = GetAmmoCount();

            for (int i = 0; i < ammo; i++)
            {
                _ = DOVirtual.DelayedCall(i * 0.125F, () =>
                {
                    var projectile = m_projectilePool.Get();
                    SetupProjectile(projectile);
                }).SetId(this);
            }
        }

        public override void Move(Vector2 direction)
        {
        }

        public override void Rotate(Vector2 direction)
        {
            // Only continues if direction is not zero
            if (direction.Equals(default)) return;

            // Get angle from direction
            // Clamp the angle to 0-180 degrees
            var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -90, 90);

            // Smoothly rotates the cannon to avoid jerky movements or instant rotations
            transform.localRotation =
                Quaternion.Slerp(transform.localRotation,
                    Quaternion.Euler(0, angle, 0), Time.deltaTime * 1.5F);
        }

        protected override void OnDeath()
        {
        }

        // Unity Methods -----------------------------------------------------

        protected override void Awake()
        {
            // Get the projectile reference
            m_projecRef = Addressables.LoadAssetAsync<GameObject>("Projectile")
                .WaitForCompletion()
                .GetComponent<Projectile.Projectile>();

            // Create a pool of projectiles
            m_projectilePool = new ObjectPool<Projectile.Projectile>(
                CreatePooleableProjectile, OnTakeFromPool,
                OnReturnedToPool, OnDestroyPoolObject,
                false, 10, 20
            );

            base.Awake();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var cannonPos = transform.position;
            var cannonForward = transform.forward;

            // Draw the cannon
            Gizmos.color = Color.red;
            Gizmos.DrawRay(cannonPos, cannonForward * 5);

            // Draw cannon rotation arc limits
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(cannonPos, Quaternion.Euler(0, -90, 0) * Vector3.forward * 5);
            Gizmos.DrawRay(cannonPos, Quaternion.Euler(0, 90, 0) * Vector3.forward * 5);

            // draw arc, connecting the previous ray to the next one
            for (var i = 0; i < 180; i++)
            {
                Gizmos.DrawLine(
                    Quaternion.Euler(0, -90 + i, 0) * Vector3.forward * 5 + cannonPos,
                    Quaternion.Euler(0, -90 + i + 1, 0) * Vector3.forward * 5 + cannonPos);
            }

            // Handles text to display the current angle of the cannon
            var angle = Mathf.Atan2(cannonForward.x, cannonForward.z) * Mathf.Rad2Deg;

            // draw handles label angle on the middle of the arc
            UnityEditor.Handles.Label(cannonPos + Quaternion.Euler(0, angle, 0) * Vector3.forward * 5.5F,
                $"{angle:0}°");
        }

        void OnGUI()
        {
            var position = transform.position;
        }
#endif

        // Private Methods ---------------------------------------------------

        /// <summary>
        /// Fires a projectile
        /// </summary>
        /// <param name="callback"></param>
        internal void OnFire(InputAction.CallbackContext callback) => Attack();
        
        private Projectile.Projectile CreatePooleableProjectile()
        {
            // Clone the projectile prefab
            var projectile = Instantiate(m_projecRef);

            // Set the projectile release method
            projectile.OnRelease += () => m_projectilePool.Release(projectile);

            // Return the projectile
            return projectile;
        }

        private void OnReturnedToPool(Projectile.Projectile projectile)
        {
            // Set the projectile to inactive
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(Projectile.Projectile projectile)
        {
            // Destroy the projectile
            Destroy(projectile.gameObject);
        }

        private void OnTakeFromPool(Projectile.Projectile projectile)
        {
            // Set the projectile to active
            projectile.gameObject.SetActive(true);
        }

        private void SetupProjectile(Projectile.Projectile projectile)
        {
            var trans = transform;
            var forward = trans.forward;

            // Set projectile position and rotation
            projectile.transform.position = (forward * 2F) + trans.position;

            // Set projectile forwad to cannon forward
            projectile.Launch(forward, GetAmmoType.Invoke());

            projectile.gameObject.SetActive(true);
        }
    }
}