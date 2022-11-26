using System;
using Enums;
using Modules.Factory;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Collectable
{
    public class CollectableFactory : TrashTypesFactory<Collectable>
    {
        public Collectable CreateRandomCollectable()
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
            var obj = GetObject();
            obj.SetType(type);

            obj.transform.position = center + Vector3.up * 2;
            obj.gameObject.SetActive(true);
        }
    }
}