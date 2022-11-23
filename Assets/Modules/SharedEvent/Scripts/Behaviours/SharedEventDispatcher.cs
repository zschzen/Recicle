using UnityEngine;

namespace Modules.SharedEvent.Behaviours
{
    public class SharedEventDispatcher : MonoBehaviour
    {
        [SerializeField] private SharedEvent m_sharedEvent;

        /// <summary>
        /// Dispatches the event.
        /// </summary>
        public void Dispatch()
        {
            m_sharedEvent.Dispatch();
        }
    }
}