using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enums;
using Modules.BucketUpdate.Script;
using Modules.Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Enemy
{
    public class Enemy : ABaseCharacter<SOEnemyData>, IBatchLateUpdate
    {
        // Static, Readonly, Const -----------------------------------------------------------

        /// <summary>
        /// Function that returns a random collectable type
        /// </summary>
        public event Action<Enemy> OnDropCollectable;

        // Keep track of the closest enemy to the player
        public static readonly Dictionary<Enemy, float> sr_enemiesDistanceToPlayer = new();

        private static ABaseCharacter<SOCharacterData> s_collectorCharacter;
        private static ABaseCharacter<SOCharacterData> s_cityCharacter;
        private static LayerMask? s_playerLayerMask;

        // Structs, Enums ------------------------------------------------------
        public enum State
        {
            MoveToCity,
            MoveToPlayer,
            Attack
        }

        // Fields ---------------------------------------------------------------------------

        [field: SerializeField] public DiscardTypes Type { get; private set; }
        [field: SerializeField] public State CurrentState { get; private set; } = State.MoveToCity;
        [SerializeField] private CharacterController m_characterController;

        private RaycastHit[] m_hits = new RaycastHit[1];
        private bool m_canAttack = true;

        private bool bIsInRange(Component target, float range, out float distance) =>
            (distance = (transform.position - target.transform.position).magnitude) < range;

        // Public methods -------------------------------------------------------------------

        public void SetType(DiscardTypes type)
        {
            if (Type != DiscardTypes.None) return;

            Type = type;
        }

        public override void TakeDamage(int damage, DiscardTypes type)
        {
            // Organic only takes damage from recyclable and vice versa
            switch (type)
            {
                case DiscardTypes.NonRecyclable when DiscardTypes.NonRecyclable.HasFlag(this.Type):
                case DiscardTypes.Recyclable when DiscardTypes.Recyclable.HasFlag(this.Type):
                    return;
            }

            if (IsDead) return;

            // Apply damage
            Health.value -= damage;

            // Check death
            if (!IsDead) return;
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
            var rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y));

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15F);
        }

        public override void Attack()
        {
            if (!m_canAttack) return;
            m_canAttack = false;

            // Try to hit a player character
            if (!TryCastCharacter(out var character)) return;

            // Inflict damage
            character.TakeDamage(1, Type);

            m_canAttack = false;

            // Wait for next attack
            _ = DOVirtual.DelayedCall(2.75F, () => m_canAttack = true).SetId(GetInstanceID());
        }

        protected override void OnDeath()
        {
            UpdateManager.Instance.DeregisterLateUpdate(this);

            // Animate death, them release
            _ = transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                // Restore the scale
                transform.localScale = Vector3.one;

                gameObject.SetActive(false);
            }).SetId(GetInstanceID());

            // Calculates the drop chance on death
            if (Random.value > CharacterData.DropChance) return;

            // Drop a collectable
            OnDropCollectable?.Invoke(this);
        }

        public void BatchLateUpdate()
        {
            CurrentState = DecideNextState();

            // Act according to state
            switch (CurrentState)
            {
                case State.MoveToPlayer:
                    PursueTarget(s_collectorCharacter);
                    break;
                case State.MoveToCity:
                    PursueTarget(s_cityCharacter);
                    break;
                case State.Attack:
                    Attack();
                    break;
            }
        }

        public void BatchFixedUpdate()
        {
        }

        // Unity Methods --------------------------------------------------------------------

        protected override void Awake()
        {
            base.Awake();

            s_playerLayerMask ??= LayerMask.GetMask("Collectors") | LayerMask.GetMask("City");

            // Find player and city
            s_collectorCharacter ??= GameObject.FindWithTag("Player").GetComponent<ABaseCharacter<SOCharacterData>>();
            s_cityCharacter ??= GameObject.FindWithTag("City").GetComponent<ABaseCharacter<SOCharacterData>>();
        }

        private void OnEnable()
        {
            // Register slice update
            UpdateManager.Instance.RegisterLateUpdateSliced(this, 0);

            // Sets the initial state
            CurrentState = State.MoveToCity;

            // Reset health
            Health.value = MaxHealth;

            // Register as active enemy
            sr_enemiesDistanceToPlayer.Add(this, float.MaxValue);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            // Deregister slice update
            UpdateManager.Instance.DeregisterLateUpdate(this);

            // Kills any tween
            _ = DOTween.Kill(GetInstanceID());

            // Release the enemy
            OnRelease?.Invoke();

            // Deregister as active enemy
            sr_enemiesDistanceToPlayer.Remove(this);

            // Reset type
            Type = DiscardTypes.None;
        }

#if UNITY_EDITOR
        protected override string GetDebugString()
        {
            s_stringBuilder.Clear();
            s_stringBuilder.Append("Health: ").AppendLine(Health.value.ToString());
            s_stringBuilder.Append("State: ").AppendLine(CurrentState.ToString());
            s_stringBuilder.Append("Type: ").AppendLine(Type.ToString());

            return s_stringBuilder.ToString();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // Draw attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, CharacterData.AttackRange);
        }
#endif

        // Private Methods ------------------------------------------------------------------

        /// <summary>
        /// Tries to cube cast a valid <see cref="ABaseCharacter{T}"/> in the forward direction
        /// </summary>
        /// <param name="character">OUT: The character that was hit, if any</param>
        /// <returns>True if a valid and alive <see cref="ABaseCharacter{T}"/> was found</returns>
        private bool TryCastCharacter(out ABaseCharacter<SOCharacterData> character) =>
            !TryCubeCast(s_playerLayerMask, ref m_hits, out var hit)
                ? (bool)(character = default)
                : hit.collider.TryGetComponent(out character) && character.enabled && !character.IsDead;

        /// <summary>
        /// Gets the n closest enemies to the player.
        /// Since is a simple game with few enemies, this is a simple implementation.
        /// Consider using QuadTrees or other data structures for a more complex game.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private Enemy[] GetClosestEnemies(int count) =>
            sr_enemiesDistanceToPlayer
                .OrderBy(pair => pair.Value)
                .Select(pair => pair.Key)
                .Take(count)
                .ToArray();

        /// <summary>
        /// Decides the next state of the enemy.
        /// </summary>
        /// <returns></returns>
        private State DecideNextState()
        {
            // if no player is never found, move to city
            if (s_collectorCharacter.IsDead) return State.MoveToCity;

            // If the player is in range, attack
            if (bIsInRange(s_collectorCharacter, CharacterData.AttackRange, out var distance))
            {
                // Update the distance to the player
                sr_enemiesDistanceToPlayer[this] = distance;
                DeterminesPlayerAttack();
            }

            // If the player is in range, move to player
            if (bIsInRange(s_collectorCharacter, CharacterData.InteractionRange, out distance))
            {
                // Update the distance to the player
                sr_enemiesDistanceToPlayer[this] = distance;
                return State.MoveToPlayer;
            }

            // If the city is in range, move to city
            if (bIsInRange(s_cityCharacter, CharacterData.InteractionRange, out var cityDistance))
                return State.MoveToCity;

            // Otherwise, move to the closest player
            if (GetClosestEnemies(2).Length < 2) return State.MoveToCity;

            return distance < cityDistance ? State.MoveToPlayer : State.MoveToCity;
        }

        private void DeterminesPlayerAttack()
        {
            // Check if its the closest one
            if (!m_canAttack) return;
            if (!this.Equals(GetClosestEnemies(1)[0])) return;

            Attack();

            // Ignores the player
            if (!s_collectorCharacter) return;

            // TODO: Animates the attack. For now, just a simple sequence, delay...
            var sequence = DOTween.Sequence().SetId(GetInstanceID());

            sequence.Append(transform.DOScale(Vector3.one * 1.25F, 0.25F));
            sequence.Join(s_collectorCharacter.transform.DOScale(Vector3.zero, 0.25F));
            sequence.Join(s_collectorCharacter.transform.DOMove(transform.position, 0.25F));
            sequence.AppendInterval(0.75F);
            sequence.Append(transform.DOScale(Vector3.one, 0.25F));
            sequence.Join(s_collectorCharacter.transform.DOScale(Vector3.one, 0.25F));
            sequence.Join(LaunchesPlayerAfterAttack());
        }

        private void PursueTarget(Component target)
        {
            // Get the direction to the target
            var direction = target.transform.position - transform.position;
            // Adjust vector to 2D
            direction.y = direction.z;

            Move(direction);
            Rotate(direction);
        }

        private Tween LaunchesPlayerAfterAttack()
        {
            // Launch the player in the opposite direction of the attack using tween
            var direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

            // Launch the player
            return s_collectorCharacter.transform.DOMove(s_collectorCharacter.transform.position + direction * 5, 0.5f);
        }
    }
}