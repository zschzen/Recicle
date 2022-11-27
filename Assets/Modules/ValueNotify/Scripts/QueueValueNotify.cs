using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Modules.ValueNotify
{
    [Serializable]
    public class QueueValueNotify<T> : IEnumerable
    {
        protected Queue<T> _value;

        public Action OnChange = delegate { };

        public int Length => _value.Count;

        public QueueValueNotify()
        {
            _value = new Queue<T>();
        }

        public QueueValueNotify(IEnumerable<T> collection)
        {
            _value = new Queue<T>(collection);
        }

        public Queue<T> value
        {
            protected get => _value;
            set
            {
                _value = value;
                OnChange.Invoke();
            }
        }

        public T Peek() => _value.Peek();

        public void Enqueue(T itemToAdd)
        {
            // Add the item to the queue
            _value.Enqueue(itemToAdd);
            OnChange.Invoke();
        }

        public T Dequeue()
        {
            // Remove the item from the queue
            var item = _value.Dequeue();
            OnChange.Invoke();
            return item;
        }

        public void Empty()
        {
            // Empty the queue
            _value.Clear();
            OnChange.Invoke();
        }

        public virtual void RemoveAt(int index)
        {
            // Remove the item at the index
            _value = new Queue<T>(_value.Where((item, i) => i != index));
            OnChange.Invoke();
        }

        public virtual void Shuffle()
        {
            // Shuffle the queue
            var random = new Random();
            _value = new Queue<T>(_value.OrderBy(x => random.Next()));
            OnChange.Invoke();
        }

        public void ForEach(Action<T> step)
        {
            foreach (var t in _value)
                step(t);
        }

        public bool Contains(T item) => _value.Contains(item);

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var item in _value)
                sb.Append(item).Append(", ");
            return sb.ToString();
        }

        public IEnumerator GetEnumerator()
        {
            return _value.GetEnumerator();
        }
    }
}