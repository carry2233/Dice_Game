using UnityEngine;

public class SpawnOnDisable : MonoBehaviour
{
    [Header("비활성화 시 생성할 프리팹")]
    public GameObject prefabToSpawn;

    [Header("회전 설정")]
    public bool copyRotation = true;

    [Header("딜레이 설정")]
    public float disableSpawnBlockTime = 3f; // ✅ 게임 시작 후 해당 시간 동안 소환 비활성화
    private float startTime;

    private void Start()
    {
        startTime = Time.time; // ✅ 게임 시작 시간 기록
    }

    private void OnDisable()
    {
        // ✅ 설정된 시간 이전이면 소환 막기
        if (Time.time - startTime < disableSpawnBlockTime)
        {
            Debug.Log($"[SpawnOnDisable] {disableSpawnBlockTime}초 동안 소환이 막혀 있습니다.");
            return;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[SpawnOnDisable] 할당된 프리팹이 없습니다.");
            return;
        }

        if (SpawnOnDamagePoolManager.Instance == null)
        {
            Debug.LogWarning("[SpawnOnDisable] SpawnOnDamagePoolManager 인스턴스가 존재하지 않습니다.");
            return;
        }

        Vector3 spawnPosition = transform.position;
        Quaternion spawnRotation = copyRotation ? transform.rotation : Quaternion.identity;

        SpawnOnDamagePoolManager.Instance.SpawnFromPool(prefabToSpawn, spawnPosition, spawnRotation);
    }
}
