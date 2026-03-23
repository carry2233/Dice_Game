using UnityEngine;
using System.Collections.Generic;

public class MonsterCreation : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn; // 생성할 몬스터 프리팹
    public Transform spawnLocation; // 몬스터 생성 위치
    public float spawnInterval = 5f; // 몬스터 생성 주기 (초)
    public int poolSize = 10; // 오브젝트 풀 크기

    private Queue<GameObject> objectPool = new Queue<GameObject>(); // 오브젝트 풀

    private void Start()
    {
        // 오브젝트 풀 초기화
        InitializeObjectPool();

        // 주기적으로 몬스터 생성
        InvokeRepeating(nameof(SpawnMonster), 0f, spawnInterval);
    }

    /// <summary>
    /// 오브젝트 풀 초기화
    /// </summary>
    private void InitializeObjectPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefabToSpawn);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    /// <summary>
    /// 몬스터 생성
    /// </summary>
    private void SpawnMonster()
    {
        GameObject monster = GetInactiveObjectFromPool();

        if (monster == null)
        {
            // 풀에 비활성화된 오브젝트가 없을 경우 새로 생성
            monster = Instantiate(prefabToSpawn);
            Debug.Log("[MonsterCreation] Created new monster as no inactive object was available in the pool.");
        }
        else
        {
            Debug.Log("[MonsterCreation] Reused inactive object from the pool.");
        }

        // 몬스터 위치 및 활성화 처리
        monster.transform.position = spawnLocation.position;
        monster.transform.rotation = Quaternion.identity;
        monster.SetActive(true);
    }

    /// <summary>
    /// 풀에서 비활성화된 오브젝트 가져오기
    /// </summary>
    /// <returns>비활성화된 오브젝트 또는 null</returns>
    private GameObject GetInactiveObjectFromPool()
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}
