using UnityEngine;

namespace Modules.Character
{
    public abstract class ABaseCharacter : MonoBehaviour
    {
        // Properties ------------------------------
        [field: SerializeField] public SOCharacterData CharacterData { get; protected set; }
        [field: SerializeField] public int Health { get; protected set; }
        [field: SerializeField] public float Speed { get; protected set; }

        // Fields ----------------------------------

        protected int MaxHealth => CharacterData.MaxHealth;
        protected float MaxSpeed => CharacterData.MaxSpeed;

        /// <summary>
        /// Determines if the character is dead.
        /// </summary>
        protected bool bIsDead => Health < 1;

        // Public Methods --------------------------------------

        public virtual void SetData(SOCharacterData data)
        {
            if (data == default) return;

            CharacterData = data;

            Health = data.MaxHealth;
            Speed = data.MaxSpeed;
        }

        /// <summary>
        /// Inflict damage to the character
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TakeDamage(int damage)
        {
            if (bIsDead) return;

            // Apply damage
            Health -= damage;

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
            if (CharacterData == default) return;

            // Spawn the character body
            var body = Instantiate(CharacterData.Body, transform);
            body.transform.localPosition = Vector3.zero;
        }
    }
}