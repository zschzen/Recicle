using Enums;
using Modules.Factory;
using UnityEngine;

namespace Modules.Projectile
{
    public class ProjectileFactory : TrashTypesFactory<Projectile>
    {
        public Projectile GetObject(DiscardTypes type)
        {
            var obj = GetObject();
            obj.SetType(type);

            UpdateVisuals(obj);

            return obj;
        }

        private void UpdateVisuals(Projectile obj)
        {
            var data = GetData().GetByType(obj.Type);
            obj.GetComponent<Renderer>().material.color = data.Color;
        }
    }
}