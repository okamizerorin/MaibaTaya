using UnityEngine;
using System.Collections.Generic;

public class ObstaclePooler : MonoBehaviour
{
    public static ObstaclePooler Instance;
    private Dictionary<string, Queue<GameObject>> poolDictionary = 
        new Dictionary<string, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string key = prefab.name;
        if (!poolDictionary.ContainsKey(key))
            poolDictionary.Add(key, new Queue<GameObject>());

        GameObject objToSpawn;

        if (poolDictionary[key].Count == 0)
        {
            objToSpawn = Instantiate(prefab);
            objToSpawn.name = key;
        }
        else
        {
            objToSpawn = poolDictionary[key].Dequeue();
        }

        objToSpawn.transform.position = position;
        objToSpawn.transform.rotation = rotation;
        objToSpawn.SetActive(true);

        return objToSpawn;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        if (poolDictionary.ContainsKey(obj.name))
        {
            poolDictionary[obj.name].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}