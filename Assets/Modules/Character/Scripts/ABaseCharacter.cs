using Enums;
using UnityEngine;

namespace Modules.Character
{
    public abstract class ABaseCharacter<T> : MonoBehaviour where T : SOCharacterData
    {
        // Properties ------------------------------
        [field: SerializeField] public T CharacterData { get; protected set; }

        // Fields -----------------------------------

        public float SpeedPerFrame => CharacterData.Speed * Time.deltaTime;

        protected int MaxHealth => CharacterData.MaxHealth;
        protected float MaxSpeed => CharacterData.MaxSpeed;

        /// <summary>
        /// Determines if the character is dead.
        /// </summary>
        protected bool bIsDead => CharacterData.Health < 1;

        // Public Methods --------------------------------------

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

        // Private Methods -------------------------------------

        protected virtual bool TryCubeCast(LayerMask layerMask, ref RaycastHit[] rayHits, out RaycastHit hit)
        {
            // Tries to cast a non alloc box cast
            var hits = Physics.BoxCastNonAlloc(transform.position, Vector3.one,
                transform.forward, rayHits, transform.rotation, CharacterData.InteractionRange, layerMask);

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
            return;
            if (CharacterData == default) return;

            // Spawn the character body
            var body = Instantiate(CharacterData.Body, transform);
            body.transform.localPosition = Vector3.zero;
        }
    }
}