using DG.Tweening;
using Enums;
using UnityEngine;

namespace Modules.Factory
{
    public abstract class TrashTypesFactory<T> : Factory<T, SOTypeFactory> where T : FactoryBehaviour
    {
        public virtual T GetAndSetType(SOTypeFactory type)
        {
            return GetObject();
        }


        /// TODO: Theres must be a way to automatic distinguish between base values and flag ones!
        /// <summary>
        /// Get random base values, such as <see cref="DiscardTypes.Organic"/>,
        /// <see cref="DiscardTypes.Plastic"/>, <see cref="DiscardTypes.Paper"/>
        /// and <see cref="DiscardTypes.Glass"/>
        /// </summary>
        /// <returns></returns>
        public static DiscardTypes GetRandomBaseType() => (DiscardTypes)Random.Range(1, 3);

        /// <summary>
        /// Get random flag values, such as <see cref="DiscardTypes.Recyclable"/>, <see cref="DiscardTypes.NonRecyclable"/>
        /// </summary>
        /// <returns></returns>
        public static DiscardTypes GetRandomFlagType() => (DiscardTypes)Random.Range(4, 5);

        /// <summary>
        /// Get any random value
        /// </summary>
        /// <returns></returns>
        public static DiscardTypes GetRandomType() => (DiscardTypes)Random.Range(1, 5);
    }
}