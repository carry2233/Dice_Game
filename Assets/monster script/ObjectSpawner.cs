using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [Header("생성 위치 (할당된 오브젝트의 위치를 사용)")]
    public Transform spawnPointObject;

    [Header("생성 주기 설정 (초)")]
    public float spawnInterval = 2f;

    [Header("소환할 프리팹 리스트")]
    public List<GameObject> spawnList = new List<GameObject>();

    private Coroutine spawnCoroutine;

    private void OnEnable()
    {
        // 활성화될 때만 코루틴 시작
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    private void OnDisable()
    {
        // 비활성화되면 코루틴 멈춤
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomObject();
        }
    }

    private void SpawnRandomObject()
    {
        if (spawnList.Count == 0 || spawnPointObject == null)
            return;

        int randomIndex = Random.Range(0, spawnList.Count);
        GameObject prefabToSpawn = spawnList[randomIndex];

        Instantiate(prefabToSpawn, spawnPointObject.position, Quaternion.identity);
        Debug.Log($"[ObjectSpawner] {prefabToSpawn.name}이(가) {spawnPointObject.position} 위치에 소환됨");
    }
}
