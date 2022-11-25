using System;
using UnityEngine;


namespace Modules.UIScreen
{
    /// <summary>
    /// Class responsible for storing the data and notifying the when the data changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ValueNotify<T>
    {
        [SerializeField] private T m_value;

        public ValueNotify()
        {
            m_value = default;
        }

        public ValueNotify(T value) => this.value = value;

        public event Action OnChange;

        public T value
        {
            get => m_value;
            set
            {
                if (m_value?.Equals(value) == true) return;

                m_value = value;
                OnChange?.Invoke();
            }
        }

        public static implicit operator T(ValueNotify<T> d) => d.m_value;
    }
}