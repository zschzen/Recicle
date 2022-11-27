using System;
using Enums;
using Modules.Factory;
using Modules.ValueNotify;
using UnityEngine;

namespace Modules.Character
{
    public abstract class ABaseCharacter<T> : FactoryBehaviour where T : SOCharacterData
    {
        // Properties ------------------------------
        [field: SerializeField] public T CharacterData { get; protected set; }

        [field: SerializeField] public ValueNotify<int> Health { get; protected set; } = new(3);

        // Fields -----------------------------------

        public float SpeedPerFrame => CharacterData.Speed * Time.deltaTime;

        protected int MaxHealth => CharacterData.MaxHealth;
        protected float MaxSpeed => CharacterData.MaxSpeed;

        // Public Methods --------------------------------------

        /// <summary>
        /// Determines if the character is dead.
        /// </summary>
        public bool IsDead => Health < 1;

        public virtual void SetData(T data)
        {
            if (data == default) return;

            CharacterData = data;
        }

        /// <summary>
        /// Performs the attack action.
        /// </summary>
        public abstract void Attack();

        /// <summary>
        /// Inflict damage to the character
        /// </summary>
        /// <param name="damage">Damage to inflict</param>
        /// <param name="type">Type of damage</param>
        public virtual void TakeDamage(int damage, DiscardTypes type)
        {
            if (IsDead) return;

            // Apply damage
            Health.value -= damage;
        }

        /// <summary>
        /// Moves the character
        /// </summary>
        /// <param name="direction"></param>
        public abstract void Move(Vector2 direction);

        /// <summary>
        /// Rotates the character
        /// </summary>
        /// <param name="direction"></param>
        public abstract void Rotate(Vector2 direction);

        // Unity Methods --------------------------------------

        protected virtual void Awake()
        {
            SetData(CharacterData);

            // Spawn the character body
            HandleSkin();
        }

        private void OnEnable()
        {
            Health.OnChange += OnDeath_Impl;
        }

        private void OnDeath_Impl()
        {
            if (!IsDead) return;

            OnDeath();
        }

#if UNITY_EDITOR
        protected static readonly System.Text.StringBuilder s_stringBuilder = new();

        protected virtual string GetDebugString()
        {
            s_stringBuilder.Clear();

            s_stringBuilder.Append("Health: ").Append(Health).AppendLine();

            return s_stringBuilder.ToString();
        }

        protected virtual void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2.75F, GetDebugString());

            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2F);

            // Draw the forward direction
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * CharacterData.InteractionRange);
        }
#endif

        // Private Methods -------------------------------------

        protected virtual bool TryCubeCast(LayerMask? layerMask, ref RaycastHit[] rayHits, out RaycastHit hit)
        {
            // Tries to cast a non alloc box cast
            var hits = Physics.BoxCastNonAlloc(transform.position, Vector3.one,
                transform.forward, rayHits, transform.rotation, CharacterData.InteractionRange, (LayerMask)layerMask);

            // Only continues if there is a hit
            if (hits == 0)
            {
                hit = default;
                return false;
            }

            // Returns the first hit
            hit = rayHits[0];
            return true;
        }

        /// <summary>
        /// Deal with death.
        /// <br/>
        /// <value>Se te queres matar, porque não te queres matar?</value>
        /// </summary>
        protected abstract void OnDeath();

        /// <summary>
        /// Deals with spawning the character skin.
        /// </summary>
        protected virtual void HandleSkin()
        {
        }
    }
}