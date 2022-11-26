using Enums;
using Modules.Factory;
using UnityEngine;

namespace Modules.Enemy
{
    public class EnemyFactory : TrashTypesFactory<Enemy>
    {
        public Enemy GetAndSetType(DiscardTypes type)
        {
            var obj = base.GetObject();
            obj.SetType(type);
            var data = GetData().GetByType(obj.Type);

            obj.GetComponent<Renderer>().material.color = data.Color;

            return obj;
        }
    }
}