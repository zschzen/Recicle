using Enums;
using Modules.Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class CollectorController : ABaseCharacter<SOCharacterData>
    {
        [SerializeField] private CharacterController m_characterController;

        private RaycastHit[] m_hits = new RaycastHit[1];
        private Collectable.Collectable m_collectable;

        public override void Attack()
        {
        }

        public override void Move(Vector2 direction)
        {
            // Only continues if direction is not zero
            if (direction.Equals(default)) return;

            // Move the body to the direction
            var moveDirection = new Vector3(direction.x, 0, direction.y);
            m_characterController.Move(moveDirection * SpeedPerFrame);

            // smooth rotation
            var rotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15F);
        }

        public override void Rotate(Vector2 direction)
        {
        }

        protected override void OnDeath()
        {
        }

        // Unity Methods -----------------------------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Draw a sphere to represent interaction range onto body controller
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * CharacterData.InteractionRange);
        }
#endif

        // Private Methods ----------------------------------------------------------------------------------------------
        /// <summary>
        /// Tries to collect an item, if there is one in range
        /// TODO: Player must carry the collected item to the correct place
        /// </summary>
        /// <param name="callback"></param>
        internal void OnCollect(InputAction.CallbackContext callback)
        {
            // Only continues if its dead or a simple tap
            if (bIsDead) return;

            // Tries to collect an item or dispose
            if (!m_collectable) TryAttachItem();
            else TryDisposeToContainer();
        }

        /// <summary>
        /// Tries to find and attach an item to the collector body.
        /// </summary>
        private void TryAttachItem()
        {
            if (!TryCubeCast(LayerMask.GetMask("Collectable"), ref m_hits, out var hit)) return;

            // If the hit has an item component
            if (!hit.collider.TryGetComponent(out Collectable.Collectable item)) return;

            m_collectable = item;

            // attach the item to the transform
            item.transform.SetParent(transform, true);
        }

        /// <summary>
        /// Tries to find a container to dispose the item.
        /// </summary>
        private void TryDisposeToContainer()
        {
            if (!TryCubeCast(LayerMask.GetMask("Container"), ref m_hits, out var hit)) return;

            // If the hit has an Container component
            if (!hit.collider.TryGetComponent(out Container container)) return;

            // check if container type accepts the item
            if (!container.Type.HasFlag(m_collectable.Type)) return;

            // Dispose the item to the container
            container.Dispose(m_collectable);

            // Destroy the item
            Destroy(m_collectable.gameObject);
        }
    }
}