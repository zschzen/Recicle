using UnityEngine;
using System;

namespace Modules.SharedEvent.Types
{
    [Serializable, CreateAssetMenu(menuName = "Events/ScriptableObject")]
    public class ScriptableObjectSharedEvent : ASharedEventGeneric<ScriptableObject>
    {
    }
}