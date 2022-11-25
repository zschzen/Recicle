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

        private RaycastHit[] m_hits = new RaycastHit[1];

        private bool bIsInRange(Transform target, float range, out float distance) =>
            (distance = (transform.position - target.position).magnitude) < range;

        // Public methods -------------------------------------------------------------------

        public override void TakeDamage(int damage, DiscardTypes type)
        {
            // Organic only takes damage from recyclable and vice versa
            switch (type)
            {
                case DiscardTypes.Organic when CharacterData.Type.HasFlag(DiscardTypes.Organic):
                case DiscardTypes.Recyclable when CharacterData.Type.HasFlag(DiscardTypes.Recyclable):
                    return;
            }

            if (bIsDead) return;

            // Apply damage
            CharacterData.Health.value -= damage;

            // Check death
            if (!bIsDead) return;
            OnDeath();
        }

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
            // Tries to get Collectors player body
            if (TryCubeCast(LayerMask.GetMask("Collectors"), ref m_hits, out var hit))
            {
                if (hit.collider.TryGetComponent<ABaseCharacter<SOCharacterData>>(out var collector))
                {
                    collector.TakeDamage(1, Type);
                    return;
                }
            }

            // Try to get city
            if (!TryCubeCast(LayerMask.GetMask("City"), ref m_hits, out hit)) return;
            if (!hit.collider.TryGetComponent<ABaseCharacter<SOCharacterData>>(out var city)) return;

            city.TakeDamage(1, Type);
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

#if UNITY_EDITOR
        private static readonly System.Text.StringBuilder s_stringBuilder = new();

        private void OnDrawGizmos()
        {
            s_stringBuilder.Clear();
            s_stringBuilder.Append("Health: ").AppendLine(CharacterData.Health.value.ToString());
            s_stringBuilder.Append("State: ").AppendLine(CurrentState.ToString());
            s_stringBuilder.Append("Type: ").AppendLine(Type.ToString());
            s_stringBuilder.Append("IsDead: ").AppendLine(bIsDead.ToString());
            
            // set handle color
            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.75F, s_stringBuilder.ToString());
        }
#endif

        // Private Methods ------------------------------------------------------------------

        private void Think()
        {
            // if city or player in range, set state to attack
            if (bIsInRange(s_cityTransform, CharacterData.AttackRange, out var cityDist) ||
                bIsInRange(s_playerTransform, CharacterData.AttackRange, out var playerDist))
            {
                CurrentState = State.Attack;
                return;
            }

            // Move to the closest target
            CurrentState = cityDist < playerDist ? State.MoveToCity : State.MoveToPlayer;
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