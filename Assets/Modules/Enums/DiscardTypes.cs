using System;

namespace Enums
{
    [Flags]
    public enum DiscardTypes
    {
        None = 0,
        Organic = 1,
        Metalic = 2,
        Plastic = 4,


        // Recyclable is metalic or plastic, but not organic
        Recyclable = Metalic | Plastic,

        // NonRecyclable is organic and not recyclable 
        NonRecyclable = Organic | ~Recyclable
    }
}