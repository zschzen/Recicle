using System.Collections.Generic;
using Enums;
using Modules.ValueNotify;
using UnityEngine;

namespace Modules.Player
{
    public class Ammo
    {
        // Fields --------------------------------------------------------------------------------------------------------------------------------

        private Dictionary<DiscardTypes, QueueValueNotify<int>> m_clips;
        private DiscardTypes m_currentAmmoType = DiscardTypes.Recyclable;

        // Properties --------------------------------------------------------------------------------------------------------------------------------
        internal IReadOnlyDictionary<DiscardTypes, QueueValueNotify<int>> Clips => m_clips;

        // Public Methods ----------------------------------------------------------------------------------------------------------------------------------

        public Ammo()
        {
            m_clips = new Dictionary<DiscardTypes, QueueValueNotify<int>>
            {
                { DiscardTypes.Recyclable, new QueueValueNotify<int>() },
                { DiscardTypes.NonRecyclable, new QueueValueNotify<int>() }
            };
        }

        public int PeekAmmo(DiscardTypes ammoType) =>
            m_clips.TryGetValue(ammoType, out var ammo) ? ammo.Peek() : default;

        /// <summary>
        /// Sets the current ammo type to the given type.
        /// </summary>
        /// <param name="ammoType"></param>
        public void SetAmmoType(DiscardTypes ammoType) => m_currentAmmoType = ammoType;

        /// <summary>
        /// Determines whether the player has ammo or not.
        /// </summary>
        /// <returns>Has ammo?</returns>
        public bool HasAmmo()
        {
            var ammo = m_clips[m_currentAmmoType];

            // Check if ammo is not empty or if not with value of zero
            return ammo.Length > 0 && ammo.Peek() > 0;
        }

        /// <summary>
        /// Adds the given ammo type with the given amount to the player's ammo.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        public void AddAmmo(DiscardTypes type, int amount)
        {
            m_clips[type].Enqueue(amount);
        }

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