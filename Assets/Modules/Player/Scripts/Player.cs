using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Modules.Character;
using UnityEngine.InputSystem;

namespace Modules.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class Player : ABaseCharacter<SOCharacterData>
    {
        [SerializeField] private PlayerInput m_playerInput;

        [SerializeField] private CannonController m_cannonController;
        [SerializeField] private CollectorController m_bodyController;

        private DiscardTypes m_currentAmmoType = DiscardTypes.Recyclable;

        private Dictionary<DiscardTypes, Queue<int>> m_ammo =
            new()
            {
                { DiscardTypes.Recyclable, new Queue<int>() },
                { DiscardTypes.Organic, new Queue<int>() }
            };

        public override void Attack()
        {
        }

        /// <summary>
        /// Implementation used to only rotate the cannon.
        /// </summary>
        /// <param name="direction"></param>
        public override void Move(Vector2 direction) => m_cannonController.Rotate(direction);

        /// <summary>
        /// Implementation used to move and rotate the body of the collector player
        /// </summary>
        /// <param name="direction"></param>
        public override void Rotate(Vector2 direction) => m_bodyController.Move(direction);

        protected override void OnDeath()
        {
        }

        // Unity Methods -----------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            // Setup cannon
            m_cannonController.GetAmmoCount = () => m_ammo[m_currentAmmoType].Dequeue();
            m_cannonController.GetAmmoType = () => m_currentAmmoType;
            m_cannonController.bHasAmmo = bHasAmmo;

            // Setup containers
            var containers = GetComponentsInChildren<Container>();
            foreach (var container in containers)
                container.OnDiscard += AddAmmo;
        }

        private void OnEnable()
        {
            // Ensure that the player input action map is set to the player
            if (!m_playerInput.currentActionMap.name.Equals("Player"))
            {
                m_playerInput.SwitchCurrentActionMap("Player");
            }

            // Subscribe to the player input events
            m_playerInput.actions["Fire"].performed += m_cannonController.OnFire;
            m_playerInput.actions["Collect"].performed += m_bodyController.OnCollect;
        }

        private void OnDisable()
        {
            // Unsubscribe to the player input events
            m_playerInput.actions["Fire"].performed -= m_cannonController.OnFire;
            m_playerInput.actions["Collect"].performed -= m_bodyController.OnCollect;
        }

        private void OnValidate()
        {
            m_cannonController ??= GetComponentInChildren<CannonController>();
            m_bodyController ??= GetComponentInChildren<CollectorController>();
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

        private bool bHasAmmo()
        {
            var ammo = m_ammo[m_currentAmmoType];

            // Check if ammo is not empty or if not with value of zero
            return ammo.Any() && ammo.Peek() > 0;
        }

        private void AddAmmo(DiscardTypes type, int count) => m_ammo[type].Enqueue(count);
    }
}