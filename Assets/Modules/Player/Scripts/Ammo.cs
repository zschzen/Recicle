using System.Collections.Generic;
using System.Linq;
using Enums;

namespace Modules.Player
{
    public class Ammo
    {
        // Fields --------------------------------------------------------------------------------------------------------------------------------

        private Dictionary<DiscardTypes, Queue<int>> m_clips;
        private DiscardTypes m_currentAmmoType = DiscardTypes.Recyclable;

        // Public Methods ----------------------------------------------------------------------------------------------------------------------------------

        public Ammo()
        {
            m_clips = new Dictionary<DiscardTypes, Queue<int>>
            {
                { DiscardTypes.Recyclable, new Queue<int>() },
                { DiscardTypes.Organic, new Queue<int>() }
            };
        }

        /// <summary>
        /// Determines whether the player has ammo or not.
        /// </summary>
        /// <returns>Has ammo?</returns>
        public bool HasAmmo()
        {
            var ammo = m_clips[m_currentAmmoType];

            // Check if ammo is not empty or if not with value of zero
            return ammo.Any() && ammo.Peek() > 0;
        }

        /// <summary>
        /// Adds the given ammo type with the given amount to the player's ammo.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        public void AddAmmo(DiscardTypes type, int amount) => m_clips[type].Enqueue(amount);

        /// <summary>
        /// Get the current ammo type.
        /// </summary>
        /// <returns></returns>
        public DiscardTypes GetCurrentAmmoType() => m_currentAmmoType;

        /// <summary>
        /// Retrieves the current ammo amount, spending it.
        /// </summary>
        /// <returns></returns>
        public int RetrieveAmmoCount() => m_clips[m_currentAmmoType].Dequeue();
    }
}