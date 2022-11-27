using System;
using Enums;
using Modules.Factory;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Collectable
{
    public class Collectable : FactoryBehaviour
    {
        [field: SerializeField] public int Size { get; private set; }
        [field: SerializeField] public DiscardTypes Type { get; private set; } = DiscardTypes.None;

        // Public methds --------------------------------------------------------

        /// <summary>
        /// Set the collectable type
        /// </summary>
        /// <param name="type"></param>
        public void SetType(DiscardTypes type)
        {
            Type = type;

            // Randomize the size of the collectable
            var possibleSizes = new[] { 2, 3, 5 };
            Size = possibleSizes[Random.Range(0, possibleSizes.Length)];
            transform.localScale = Vector3.one * (Size / 2.5F);
        }

        // Unity Methods --------------------------------------------------------------

        private void Awake() => Size = 0;

        protected void OnEnable() => transform.SetParent(default);

        protected override void OnDisable()
        {
            // Reset type
            Type = DiscardTypes.None;
            Size = 0;

            base.OnDisable();
        }
    }
}