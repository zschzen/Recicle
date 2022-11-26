using Enums;
using Modules.Collectable;
using Modules.Factory;
using UnityEngine;

namespace Modules.Enemy
{
    public class EnemyFactory : TrashTypesFactory<Enemy>
    {
        [SerializeField] CollectableFactory m_collectableFactory;

        public Enemy GetAndSetType(DiscardTypes type)
        {
            var obj = base.GetObject();
            obj.SetType(type);
            var data = GetData().GetByType(obj.Type);

            obj.GetComponent<Renderer>().material.color = data.Color;

            return obj;
        }

        // Private Methods ---------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Communicates with the CollectableFactory to spawn a collectable.
        /// </summary>
        /// <param name="enemy"></param>
        private void DropCollectable(Enemy enemy) => m_collectableFactory.Drop(enemy.transform.position, enemy.Type);

        protected override Enemy CreatePooleableObject()
        {
            var enemy = base.CreatePooleableObject();
            enemy.OnDropCollectable += DropCollectable;
            return enemy;
        }
    }
}