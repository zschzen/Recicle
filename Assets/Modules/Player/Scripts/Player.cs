using UnityEngine;
using Modules.Character;
using UnityEngine.InputSystem;

namespace Modules.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : ABaseCharacter<SOCharacterData>
    {
        [SerializeField] private PlayerInput m_playerInput;

        [SerializeField] private CharacterController m_cannonController;
        [SerializeField] private CharacterController m_bodyController;

        public override void Attack()
        {
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

        // Private Methods ----------------------------------------------------------------------------------------------

        /// <summary>
        /// Fires a projectile
        /// </summary>
        /// <param name="callback"></param>
        private void OnFire(InputAction.CallbackContext callback)
        {
            // Only continues if its dead or a simple tap
            if (bIsDead) return;
            //if (callback.interaction is not TapInteraction) return;

            // TODO: Fire
            Debug.Log("Fire");
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