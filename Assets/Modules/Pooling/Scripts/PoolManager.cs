using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Pooling
{
    /// <summary>
    /// Pooling container for a generic type
    /// </summary>
    public abstract class GenericPool<T>
    {
        public HashSet<T> Active { get; private set; }
        public HashSet<T> Inactive { get; private set; }

        /// <summary>
        /// Gets the total count of active and inactive objects
        /// </summary>
        public int Count => Active.Count + Inactive.Count;

        private T m_original;

        public GenericPool(T original)
        {
            m_original = original;
            Active = new HashSet<T>();
            Inactive = new HashSet<T>();
        }

        public GenericPool(T original, int initialSize)
        {
            m_original = original;
            Active = new HashSet<T>();
            Inactive = new HashSet<T>();

            for (int i = 0; i < initialSize; i++)
            {
                Inactive.Add(CreateNew());
            }
        }

        /// <summary>
        /// Creates a new instance of the <inheritdoc cref="T"/> type
        /// </summary>
        /// <returns></returns>
        private T CreateNew()
        {
            var obj = (T)Activator.CreateInstance(typeof(T), m_original);
            return obj;
        }

        /// <summary>
        /// Gets an object from the pool. If there are no inactive objects, a new one will be created.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Get(out T item)
        {
            if (Inactive.Count == 0)
            {
                item = default;
                return;
            }

            // if theres none, add one
            item = Inactive.Count == 0 ? CreateNew() : Inactive.First();

            Inactive.Remove(item);
            Active.Add(item);
        }

        /// <summary>
        /// Returns the item to the inactive pool
        /// </summary>
        /// <param name="item"></param>
        public virtual void Return(T item)
        {
            if (!Active.Contains(item)) return;

            Active.Remove(item);
            Inactive.Add(item);
        }
    }
}