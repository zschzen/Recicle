using UnityEngine;
using UnityEngine.Events;

namespace Modules.SharedEvent.Behaviours
{
    /// <summary>
    /// Event listener for a SharedEvent.
    /// </summary>
    public class SharedEventListener : MonoBehaviour
    {
        [SerializeField] private SharedEvent m_sharedEvent;
        [SerializeField] private UnityEvent m_callback;

        private void OnEnable()
        {
            Debug.Assert(m_sharedEvent != default, "SharedEvent is null");

            m_sharedEvent.AddListener(OnSharedEvent);
        }

        private void OnDisable()
        {
            Debug.Assert(m_sharedEvent != default, "SharedEvent is null");

            m_sharedEvent.RemoveListener(OnSharedEvent);
        }

        private void OnSharedEvent()
        {
            m_callback?.Invoke();
        }
    }
}