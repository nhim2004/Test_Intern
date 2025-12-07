using UnityEngine;

namespace WaterSort.Utilities
{
    /// <summary>
    /// Simple object pooling system for performance
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private bool expandable = true;

        private System.Collections.Generic.Queue<GameObject> pool;

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            pool = new System.Collections.Generic.Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
        }

        private GameObject CreateNewObject()
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }

        /// <summary>
        /// Get object from pool
        /// </summary>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj;

            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (expandable)
            {
                obj = CreateNewObject();
                pool.Dequeue();
            }
            else
            {
                return null;
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            return obj;
        }

        /// <summary>
        /// Return object to pool
        /// </summary>
        public void Return(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }
    }
}
