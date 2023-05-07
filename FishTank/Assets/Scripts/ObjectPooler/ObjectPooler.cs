using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    [System.Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public static ObjectPooler instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // *** Instantiate desgined objects in pool *** //
        foreach(Pool pool in pools) {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            GameObject parent = new GameObject();
            parent.name = $"{pool.tag} Parent";
            parent.transform.parent = gameObject.transform;

            for(int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab, parent.transform);
                obj.name = obj.name.Replace("(Clone)", "");
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // *** Activate object in Pool *** //
    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation) {

        if(!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning($"Pool with tag {tag} doesn't excist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if(pooledObj != null) {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

}
