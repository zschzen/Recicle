using System;
using Enums;
using Modules.Factory;
using UnityEngine;

namespace Modules.Player
{
    /// <summary>
    /// Class responsible to handle the container disposal for the cannon player.
    /// </summary>
    public class Container : MonoBehaviour
    {
        // Properties ------------------------------------------

        /// <summary>
        /// Called when the item is disposed onto the container.
        /// <param name="Type">Type of the item</param>
        /// <param name="Size">Size of the item</param>
        /// </summary>
        public event Action<DiscardTypes, int> OnDiscard;

        // Fields ----------------------------------------------

        [field: SerializeField] public DiscardTypes Type { get; private set; } = DiscardTypes.None;
        [SerializeField] private SOTypeFactory m_typeFactory;

        // Unity methods ---------------------------------------

        private void Awake()
        {
            // Sets color
            GetComponent<Renderer>().material.color = m_typeFactory.GetByType(Type).Color;
        }

        // Public Methods -------------------------------------------------------------

        public void Dispose(Collectable.Collectable collectable)
        {
            if (!collectable) return;
            OnDiscard?.Invoke(Type, collectable.Size);
        }
    }
}