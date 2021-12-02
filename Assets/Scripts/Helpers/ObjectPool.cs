using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Helpers
{
    [Serializable]
    public struct Pool
    {
        public PoolType type;
        public GameObject prefab;
        public int size;
    }

    public enum PoolType
    {
        Text,
        Soldier
    }

    public class ObjectPool : Singleton<ObjectPool>
    {
        [SerializeField]
        private List<Pool> pools;

        private readonly Dictionary<PoolType, Queue<GameObject>> poolDictionary = new Dictionary<PoolType, Queue<GameObject>>();

        private void Awake()
        {
            // Initialize Pools
            foreach (var pool in pools)
            {
                var queue = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    var obj = Instantiate(pool.prefab, gameObject.transform);
                    obj.SetActive(false);
                    queue.Enqueue(obj);
                }

                poolDictionary.Add(pool.type, queue);
            }
        }

        public GameObject SpawnFromPoolOnPosition(PoolType type, Vector3 position)
        {
            var objToSpawn = SpawnFromPool(type);
            if (!objToSpawn) return null;

            objToSpawn.transform.position = position;

            return objToSpawn;
        }

        public GameObject SpawnFromPool(PoolType type)
        {
            // Check for type
            if (!poolDictionary.ContainsKey(type))
            {
                Debug.LogError("Pool with tag " + type + " does not exist");
                return null;
            }

            // Peek next object to see if it's active
            var objToSpawn = poolDictionary[type].Peek();

            // If next object is active, instaniate new one
            if (objToSpawn.activeInHierarchy) objToSpawn = Instantiate(pools.First(p => p.type == type).prefab, gameObject.transform);
            else objToSpawn = poolDictionary[type].Dequeue();


            objToSpawn.SetActive(true);
            
            // Move to the end of queue
            poolDictionary[type].Enqueue(objToSpawn);

            return objToSpawn;
        }
    }
}