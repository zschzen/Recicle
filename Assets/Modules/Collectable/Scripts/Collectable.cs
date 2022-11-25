using Enums;
using UnityEngine;

namespace Modules.Collectable
{
    public class Collectable : MonoBehaviour
    {
        [field: SerializeField] public int Size { get; private set; } = 1;
        [field: SerializeField] public DiscardTypes Type { get; private set; } = DiscardTypes.None;
    }
}