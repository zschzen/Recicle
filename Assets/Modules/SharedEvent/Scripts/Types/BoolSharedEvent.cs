using System;
using UnityEngine;

namespace Modules.SharedEvent.Types
{
    [Serializable, CreateAssetMenu(menuName = "Events/Bool")]
    public class BoolSharedEvent : ASharedEventGeneric<bool>
    {
    }
}