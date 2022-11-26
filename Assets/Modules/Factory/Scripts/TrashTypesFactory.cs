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
    }
}