using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.SharedEvent
{
    /// <summary>
    /// Base class that implements the SharedEvent pattern.
    /// </summary>
    public abstract class ASharedEventBase<TDelegate> : ScriptableObject where TDelegate : MulticastDelegate
    {
        /// <summary>
        /// Stores the list of listeners.
        /// </summary>
        protected HashSet<TDelegate> m_listeners = new();

        /// <summary>
        /// Determines if <see cref="m_listeners"/> is empty.
        /// </summary>
        public bool bHasListeners => m_listeners.Count > 0;

        /// <summary>
        /// Dispatches the event to all listeners.
        /// </summary>
        [ContextMenu("Dispatch")]
        public abstract void Dispatch();

        /// <summary>
        /// Invokes the event.
        /// </summary>
        public virtual void Invoke() => Dispatch();
    }

    /// <summary>
    /// Class that handles the remote/shared event system trough the Scriptable Objects.
    /// <seealso cref="ASharedEventBase{MulticastDelegate}"/>
    /// </summary>
    [Serializable, CreateAssetMenu(menuName = "Events/Void")]
    public class SharedEvent : ASharedEventBase<Action>
    {
        /// <summary>
        /// Add the given listener to <see cref="m_listeners"/>
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(Action listener)
        {
            m_listeners.Add(listener);
        }

        /// <summary>
        /// Remove the given listener from <see cref="m_listeners"/>
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(Action listener)
        {
            m_listeners.Remove(listener);
        }

        /// <summary>
        /// Remove all listeners from <see cref="m_listeners"/>
        /// </summary>
        public void RemoveAllListeners()
        {
            m_listeners.Clear();
        }

        [ContextMenu("Dispatch")]
        public override void Dispatch()
        {
            if (!bHasListeners) return;

            foreach (Action listener in m_listeners)
            {
                listener?.Invoke();
            }
        }
    }

    public abstract class ASharedEventGeneric<T> : ASharedEventBase<Action<T>>
    {
        [SerializeField] protected T defaultValue;

        /// <summary>
        /// Add the given listener to <see cref="ASharedEventBase{TDelegate}.m_listeners"/>
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(Action<T> listener)
        {
            m_listeners.Add(listener);
        }

        /// <summary>
        /// Remove the given listener from <see cref="ASharedEventBase{TDelegate}.m_listeners"/>
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(Action<T> listener)
        {
            m_listeners.Remove(listener);
        }

        /// <summary>
        /// Remove all listeners from <see cref="ASharedEventBase{TDelegate}.m_listeners"/>
        /// </summary>
        public void RemoveAllListeners()
        {
            m_listeners.Clear();
        }

        /// <summary>
        /// Dispatches the event to all listeners.
        /// </summary>
        public override void Dispatch()
        {
            Dispatch(defaultValue);
        }

        /// <summary>
        /// Dispatches the event to all listeners with the given value.
        /// </summary>
        /// <param name="param">Parameter to be passed to the listeners.</param>
        public void Dispatch(T param)
        {
            if (!bHasListeners) return;

            foreach (Action<T> listener in m_listeners)
            {
                listener?.Invoke(param);
            }
        }

        /// <summary>
        /// Invokes the event with the default value.
        /// </summary>
        public override void Invoke() => Dispatch(defaultValue);

        /// <summary>
        /// Invokes the event with the given value.
        /// </summary>
        /// <param name="param">Parameter to be passed to the listeners.</param>
        public void Invoke(T param) => Dispatch(param);
    }
}