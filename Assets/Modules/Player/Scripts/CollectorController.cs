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
            Gizmos.DrawWireSphere(transform.position, CharacterData.InteractionRange);
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
            //if (callback.interaction is not TapInteraction) return;

            // do a non alloc sphere cast to check if there is an item in range
            var hits = Physics.SphereCastNonAlloc(
                transform.position,
                CharacterData.InteractionRange,
                transform.forward,
                m_hits,
                5,
                ~0 // TODO: Add layer mask
            );

            // If there is a hit
            if (hits < 1) return;

            // Get the first hit
            var hit = m_hits[0];

            // If the hit has an item component
            if (!hit.collider.TryGetComponent(out Collectable.Collectable item)) return;

            // attach the item to the transform
            item.transform.SetParent(transform, true);
        }
    }
}