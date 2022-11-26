using Modules.Factory;
using UnityEngine;

namespace Modules.Projectile
{
    public class ProjectileFactory : TrashTypesFactory<Projectile>
    {
        public override Projectile GetObject()
        {
            var obj = base.GetObject();

            var data = GetData().GetByType(obj.Type);
            obj.GetComponent<Renderer>().material.color = data.Color;

            return obj;
        }
    }
}