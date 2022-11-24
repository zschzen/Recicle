﻿using System.Text;
using UnityEngine;
using Modules.Character;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Modules.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : ABaseCharacter<SOCharacterData>
    {
        [SerializeField] private PlayerInput m_playerInput;

        [SerializeField] private CharacterController m_cannonController;
        [SerializeField] private CharacterController m_bodyController;

        private Projectile.Projectile m_projecRef;
        private IObjectPool<Projectile.Projectile> m_projectilePool;

        public override void Attack()
        {
            // Get projectile from pool
            var projectile = m_projectilePool.Get();

            // Set projectile position and rotation
            var trans = m_cannonController.transform;
            projectile.transform.position = (trans.forward * 2F) + trans.position;

            // Set projectile forwad to cannon forward
            projectile.Launch(trans.forward);

            projectile.gameObject.SetActive(true);
        }

        /// <summary>
        /// Implementation used to only rotate the cannon.
        /// </summary>
        /// <param name="direction"></param>
        public override void Move(Vector2 direction)
        {
            // Only continues if direction is not zero
            if (direction.Equals(default)) return;

            // Get angle from direction
            // Clamp the angle to 0-180 degrees
            var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -90, 90);

            // Smoothly rotates the cannon to avoid jerky movements or instant rotations
            m_cannonController.transform.localRotation =
                Quaternion.Slerp(m_cannonController.transform.localRotation,
                    Quaternion.Euler(0, angle, 0), Time.deltaTime * 1.5F);
        }

        /// <summary>
        /// Implementation used to move and rotate the body of the collector player
        /// </summary>
        /// <param name="direction"></param>
        public override void Rotate(Vector2 direction)
        {
            // Only continues if direction is not zero
            if (direction.Equals(default)) return;

            // Move the body to the direction
            var moveDirection = new Vector3(direction.x, 0, direction.y);
            m_bodyController.Move(moveDirection * SpeedPerFrame);

            // Stores the transform to avoid inefficient calls
            var bodyTransform = m_bodyController.transform;

            // smooth rotation
            var rotation = Quaternion.LookRotation(moveDirection);
            bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, rotation, 0.15F);
        }

        protected override void OnDeath()
        {
        }

        // Unity Methods -----------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            // Get the projectile reference
            m_projecRef = Addressables.LoadAssetAsync<GameObject>("Projectile")
                .WaitForCompletion()
                .GetComponent<Projectile.Projectile>();

            // Create a pool of projectiles
            m_projectilePool = new ObjectPool<Projectile.Projectile>(
                CreatePooleableProjectile, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, 10, 10);
        }

        private void OnEnable()
        {
            // Ensure that the player input action map is set to the player
            if (!m_playerInput.currentActionMap.name.Equals("Player"))
            {
                m_playerInput.SwitchCurrentActionMap("Player");
            }

            // Subscribe to the player input events
            m_playerInput.actions["Fire"].performed += OnFire;
            m_playerInput.actions["Collect"].performed += OnCollect;
        }

        private void OnDisable()
        {
            // Unsubscribe to the player input events
            m_playerInput.actions["Fire"].performed -= OnFire;
            m_playerInput.actions["Collect"].performed -= OnCollect;
        }

        private void OnValidate()
        {
            m_cannonController ??= GetComponentInChildren<CharacterController>();
            m_playerInput ??= GetComponent<PlayerInput>();
        }

        private void Update()
        {
            if (bIsDead) return;
            //if (!m_playerInput.currentActionMap.name.Equals("Player")) return; // Not needed as we are using a single action map

            // Get move value and do move
            Move(m_playerInput.actions["Move"].ReadValue<Vector2>());

            // Get rotate value and do rotate
            Rotate(m_playerInput.actions["Look"].ReadValue<Vector2>());
        }

#if UNITY_EDITOR
        private static readonly StringBuilder s_sb = new();

        private void OnDrawGizmos()
        {
            var canonTrans = m_cannonController.transform;
            var cannonPos = canonTrans.position;
            var cannonForward = canonTrans.forward;

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
            //angle = Mathf.Clamp(angle, -90, 90);

            s_sb.Clear()
                .Append($"{angle:0}°");

            // draw handles label angle on the middle of the arc
            UnityEditor.Handles.Label(cannonPos + Quaternion.Euler(0, angle, 0) * Vector3.forward * 2.5F,
                s_sb.ToString());
        }

        void OnGUI()
        {
            s_sb.Clear()
                .Append($"Pool Size: {m_projectilePool.CountInactive}");

            GUILayout.Label(s_sb.ToString());
        }
#endif

        // Private Methods ----------------------------------------------------------------------------------------------

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

        /// <summary>
        /// Fires a projectile
        /// </summary>
        /// <param name="callback"></param>
        private void OnFire(InputAction.CallbackContext callback)
        {
            // Only continues if its dead or a simple tap
            if (bIsDead) return;
            //if (callback.interaction is not TapInteraction) return;

            Attack();
        }

        /// <summary>
        /// Tries to collect an item, if there is one in range
        /// </summary>
        /// <param name="callback"></param>
        private void OnCollect(InputAction.CallbackContext callback)
        {
            // Only continues if its dead or a simple tap
            if (bIsDead) return;
            //if (callback.interaction is not TapInteraction) return;

            // TODO: Collect
            Debug.Log("Collect");
        }
    }
}