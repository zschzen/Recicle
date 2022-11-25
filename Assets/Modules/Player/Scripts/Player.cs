﻿using UnityEngine;
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

        private Ammo m_ammo = new();

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
            m_cannonController.GetAmmoCount = m_ammo.RetrieveAmmoCount;
            m_cannonController.GetAmmoType = m_ammo.GetCurrentAmmoType;
            m_cannonController.bHasAmmo = m_ammo.HasAmmo;

            // Setup containers
            var containers = GetComponentsInChildren<Container>();
            foreach (var container in containers)
                container.OnDiscard += m_ammo.AddAmmo;
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
    }
}