using System;
using Enums;
using Modules.BucketUpdate.Script;
using Modules.Character;
using UnityEngine;

namespace Modules.Enemy
{
    public class Enemy : ABaseCharacter<SOEnemyData>, IBatchLateUpdate
    {
        // Static, Readonly, Const -----------------------------------------------------------
        private static Transform s_playerTransform;
        private static Transform s_cityTransform;

        // Structs, Enums ------------------------------------------------------
        public enum State
        {
            MoveToCity,
            MoveToPlayer,
            Attack
        }

        // Properties ----------------------------------------------------------

        public event Action OnRelease;

        // Fields ---------------------------------------------------------------------------

        public DiscardTypes Type;
        [SerializeField] private CharacterController m_characterController;

        private State CurrentState;

        private bool bIsInRange(Transform target, float range) =>
            (transform.position - target.position).magnitude < range;

        // Public methods -------------------------------------------------------------------

        public override void Move(Vector2 direction)
        {
            if (direction.Equals(default)) return;

            // Move
            var moveDirection = new Vector3(direction.x, 0, direction.y);
            m_characterController.Move(moveDirection * SpeedPerFrame);

            // lock y axis
            var trans = transform;
            var pos = trans.position;

            trans.position = new Vector3(pos.x, 0, pos.z);
        }

        public override void Rotate(Vector2 direction)
        {
            if (CurrentState != State.MoveToPlayer) return;
            if (CurrentState != State.MoveToCity) return;

            var rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15F);
        }

        public override void Attack()
        {
        }

        protected override void OnDeath()
        {
            UpdateManager.Instance.DeregisterLateUpdate(this);
        }

        public void BatchLateUpdate()
        {
            // Perform state transitions
            Think();

            // Act according to state
            switch (CurrentState)
            {
                case State.MoveToPlayer:
                    PursueTarget(s_playerTransform);
                    break;
                case State.MoveToCity:
                    PursueTarget(s_cityTransform);
                    break;
                case State.Attack:
                    Attack();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Unity Methods --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            // !Moved to project settings!
            // ensures auto sync transform, due to CharacterController bug ID 1135083
            // Physics.autoSyncTransforms = true;

            // Find player and city
            s_playerTransform ??= GameObject.FindWithTag("Player").transform;
            s_cityTransform ??= GameObject.FindWithTag("City").transform;
        }

        private void OnEnable()
        {
            // Register slice update
            UpdateManager.Instance.RegisterLateUpdateSliced(this, 0);

            // Sets the initial state
            CurrentState = State.MoveToCity;
        }

        private void OnDisable()
        {
            // Deregister slice update
            UpdateManager.Instance.DeregisterLateUpdate(this);

            // Release the enemy
            OnRelease?.Invoke();
        }

        // Private Methods ------------------------------------------------------------------

        private void Think()
        {
            // Check if the player is in attack range
            // Check if the player is in interaction range
            // If not, move to the city
            CurrentState = bIsInRange(s_playerTransform, CharacterData.InteractionRange)
                ? bIsInRange(s_playerTransform, CharacterData.AttackRange) ? State.Attack : State.MoveToPlayer
                : State.MoveToCity;
        }

        private void PursueTarget(Transform target)
        {
            // Get the direction to the target
            var direction = target.position - transform.position;
            // Adjust vector to 2D
            direction.y = direction.z;

            Move(direction);
            Rotate(direction);
        }
    }
}