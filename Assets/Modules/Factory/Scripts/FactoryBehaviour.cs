using System;
using UnityEngine;

namespace Modules.Factory
{
    public abstract class FactoryBehaviour : MonoBehaviour, IFactoryObject
    {
        public Action OnRelease { get; set; }

        protected virtual void OnDisable()
        {
            // Release the projectile back to the pool
            OnRelease?.Invoke();
        }
    }
}