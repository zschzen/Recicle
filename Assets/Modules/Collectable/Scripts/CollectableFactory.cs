using Enums;
using Modules.Factory;
using UnityEngine;

namespace Modules.Collectable
{
    public class CollectableFactory : TrashTypesFactory<Collectable>
    {
        public Collectable CreateRandomTypeCollectable()
        {
            // get random type
            var type = GetRandomBaseType();
            var obj = GetObject();

            obj.SetType(type);

            SetColor(obj);

            return obj;
        }

        public override Collectable GetObject()
        {
            var obj = base.GetObject();

            SetColor(obj);

            return obj;
        }

        private void SetColor(Collectable obj)
        {
            var data = GetData().GetByType(obj.Type);
            obj.GetComponent<Renderer>().material.color = data.Color;
        }

        public void Drop(Vector3 center, DiscardTypes type)
        {
            var obj = CreateRandomTypeCollectable();
            obj.SetType(type);
            SetColor(obj);

            var pos = center + Random.insideUnitSphere * 2;
            pos.y = 0;
            obj.transform.position = pos;

            obj.gameObject.SetActive(true);
        }

        protected override void OnReturnedToPool(Collectable projectile)
        {
            base.OnReturnedToPool(projectile);
        }
    }
}