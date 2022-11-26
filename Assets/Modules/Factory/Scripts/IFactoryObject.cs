using System;

namespace Modules.Factory
{
    public interface IFactoryObject
    {
        public Action OnRelease { get; set; }
    }
}