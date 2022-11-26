using Enums;
using UnityEditor;
using UnityEngine;

namespace Modules.Factory
{
    [CreateAssetMenu(fileName = "Type Factory Data", menuName = "Gameplay/Factory/Type", order = 0)]
    public class SOTypeFactory : ASOFactoryBase
    {
        // Stores a color and a discardtype
        [System.Serializable]
        public struct TypeData
        {
            [field: SerializeField] public Color Color { get; private set; }
            [field: SerializeField] public DiscardTypes Type { get; private set; }
        }

        // List of all types
        public TypeData[] Types;

        /// <summary>
        /// Get the First TypeData with the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public TypeData GetByType(DiscardTypes type)
        {
            foreach (var typeData in Types)
            {
                if (typeData.Type.HasFlag(type))
                    return typeData;}

            return new TypeData();
        }
    }
}