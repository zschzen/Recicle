using System;
using Enums;
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

        // Public Methods -------------------------------------------------------------

        public void Dispose(Collectable.Collectable collectable) => OnDiscard?.Invoke(Type, collectable.Size);
    }
}