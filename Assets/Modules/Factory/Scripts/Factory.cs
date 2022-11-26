using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Modules.Factory
{
    public abstract class Factory<T, S> : MonoBehaviour where T : FactoryBehaviour where S : SOTypeFactory
    {
        // Addressables reference to the prefab
        [SerializeField] private AssetReference m_prefabReference;
        [SerializeField] private S m_factorySettings;

        private T m_ref;
        private IObjectPool<T> m_pool;

        public virtual S GetData() => m_factorySettings;
        public virtual T GetObject() => m_pool.Get();

        protected virtual void Awake()
        {
            m_prefabReference.LoadAssetAsync<GameObject>().Completed += OnPrefabLoaded;

            // Create a pool of projectiles
            m_pool = new ObjectPool<T>(
                CreatePooleableObject, OnTakeFromPool,
                OnReturnedToPool, OnDestroyPoolObject,
                false, 10, 20
            );
        }

        protected virtual void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj)
        {
            m_ref = obj.Result.GetComponent<T>();
        }

        protected virtual T CreatePooleableObject()
        {
            // Clone the obj prefab
            var obj = Instantiate(m_ref);

            // Set the obj release method
            obj.OnRelease += () => m_pool.Release(obj);

            // Return the obj
            return obj;
        }

        protected virtual void OnReturnedToPool(T projectile)
        {
            // Set the projectile to inactive
            projectile.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyPoolObject(T projectile)
        {
            // Destroy the projectile
            Destroy(projectile.gameObject);
        }

        protected virtual void OnTakeFromPool(T projectile)
        {
            // Set the projectile to active
            projectile.gameObject.SetActive(true);
        }
    }
}