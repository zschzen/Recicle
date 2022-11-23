using UnityEngine;
using System;

namespace Modules.SharedEvent.Types
{
    [Serializable, CreateAssetMenu(menuName = "Events/String")]
    public class StringSharedEvent : ASharedEventGeneric<string>
    {
    }
}