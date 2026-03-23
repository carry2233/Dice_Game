using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnOnDamagePoolManager : MonoBehaviour
{
    public static SpawnOnDamagePoolManager Instance;

    [System.Serializable]
    public class SpawnPool
    {
        public GameObject prefab;
        public int poolSize = 10;
        public float deactivateTime = 2f;
    }

    [Header("스폰 풀 설정")]
    public List<SpawnPool> spawnPools = new List<SpawnPool>();

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, float> deactivateTimeDict = new Dictionary<GameObject, float>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var pool in spawnPools)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary[pool.prefab] = objectQueue;
            deactivateTimeDict[pool.prefab] = pool.deactivateTime;
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning($"[SpawnPoolManager] 풀에 해당 프리팹이 없습니다: {prefab.name}");
            return null;
        }

        GameObject obj = poolDictionary[prefab].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        poolDictionary[prefab].Enqueue(obj);

        float delay = deactivateTimeDict.ContainsKey(prefab) ? deactivateTimeDict[prefab] : 2f;
        StartCoroutine(DeactivateAfterSeconds(obj, delay));

        return obj;
    }

    private IEnumerator DeactivateAfterSeconds(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
