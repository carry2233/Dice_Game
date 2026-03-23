// ✅ SceneStateManager.cs
using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager Instance;

    private Dictionary<string, ObjectStateData> savedStates = new();

private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ 씬 이동 시에도 살아 있도록!
    }
    else
    {
        Destroy(gameObject);
    }
}


public void StoreAllStates()
{
    savedStates.Clear();
    var savers = FindObjectsOfType<StateSaver>(true);

    // 종류별 프리팹 카운팅용 Dictionary
    Dictionary<string, int> prefabCount = new();

    foreach (var saver in savers)
    {
        var data = saver.SaveState();
        if (string.IsNullOrEmpty(data.uniqueID)) continue;

        savedStates[data.uniqueID] = data;

        // 프리팹 종류별 개수 카운팅
        if (!string.IsNullOrEmpty(data.prefabName))
        {
            if (!prefabCount.ContainsKey(data.prefabName))
                prefabCount[data.prefabName] = 0;
            prefabCount[data.prefabName]++;
        }

        // 개별 오브젝트 디버깅
        Debug.Log($"🟩 저장됨: {data.prefabName} ({data.uniqueID})\n" +
                  $"→ Pos: {data.position}, HP: {data.currentHealth}\n" +
                  $"→ Knockback: {data.knockbackForce}/{data.knockbackSpeedMultiplier}/{data.knockbackDuration}/{data.knockbackDelay}");
    }

    // 전체 요약 출력
    Debug.Log($"📦 총 저장된 오브젝트 수: {savedStates.Count}");

    foreach (var pair in prefabCount)
    {
        Debug.Log($"🔢 프리팹: {pair.Key} → {pair.Value}개 저장됨");
    }
}


public void RestoreAllStates()
{
    Debug.Log("🔁 복원 시도 시작");

    var registry = FindObjectOfType<MonsterPrefabRegistry>();
    if (registry == null)
    {
        Debug.LogWarning("❌ MonsterPrefabRegistry를 찾을 수 없습니다. 복원을 중단합니다.");
        return;
    }

    int successCount = 0;
    int failureCount = 0;

    foreach (var kvp in savedStates)
    {
        ObjectStateData data = kvp.Value;

        GameObject prefab = registry.GetPrefabByID(data.monsterTypeID);
        if (prefab == null)
        {
            Debug.LogWarning($"⚠️ 복원 실패: TypeID={data.monsterTypeID}, ID={data.uniqueID} → 프리팹을 찾을 수 없음");
            failureCount++;
            continue;
        }

        GameObject obj = Instantiate(prefab, data.position, data.rotation);
        obj.name = $"{prefab.name}_Restored_{data.uniqueID}";

        // 고유 ID 할당
        var idAssigner = obj.GetComponent<UniqueIDAssigner>();
        if (idAssigner != null)
        {
            idAssigner.uniqueID = data.uniqueID;
        }

        // 상태 복원
        var saver = obj.GetComponent<StateSaver>();
        if (saver != null)
        {
            saver.LoadState(data);

            // ✅ 복원 성공 로그
            Debug.Log($"✅ 복원 성공: TypeID={data.monsterTypeID}, ID={data.uniqueID}" +
                      $"\n→ 위치: {data.position}, 회전: {data.rotation.eulerAngles}" +
                      $"\n→ 체력: {data.currentHealth}");

            successCount++;
        }
        else
        {
            Debug.LogWarning($"⚠️ 복원 실패: ID={data.uniqueID} → StateSaver 없음");
            failureCount++;
        }
    }

    Debug.Log($"📦 복원 요약: 성공 {successCount}개 / 실패 {failureCount}개");
}

public void ClearAllSavedStates()
{
    if (savedStates.Count > 0)
    {
        savedStates.Clear();
        Debug.Log($"🧹 저장된 상태 초기화 완료. (삭제된 데이터 수: {savedStates.Count})");
    }
    else
    {
        Debug.Log("ℹ️ 저장된 상태가 없습니다. 초기화할 데이터 없음.");
    }
}

public void StoreWaveSpawnerState()
{
    var waveSpawner = FindObjectOfType<WaveSpawner>();
    if (waveSpawner != null)
    {
        PlayerPrefs.SetInt("WaveIndex", waveSpawner.CurrentWaveIndex);
        PlayerPrefs.SetFloat("RemainingTime", waveSpawner.RemainingTime);
        PlayerPrefs.SetFloat("SpawnTimer", waveSpawner.SpawnTimer);
        PlayerPrefs.Save();
        Debug.Log("💾 [SceneStateManager] 웨이브 스포너 상태 저장 완료");
    }
}

public void RestoreWaveSpawnerState()
{
    var waveSpawner = FindObjectOfType<WaveSpawner>();
    if (waveSpawner != null && PlayerPrefs.HasKey("WaveIndex"))
    {
        int index = PlayerPrefs.GetInt("WaveIndex");
        float remTime = PlayerPrefs.GetFloat("RemainingTime");
        float spwnTimer = PlayerPrefs.GetFloat("SpawnTimer");
        waveSpawner.SetWaveState(index, remTime, spwnTimer);
        Debug.Log("🔁 [SceneStateManager] 웨이브 스포너 상태 복원 완료");
    }
    else
    {
        Debug.Log("⚠️ [SceneStateManager] 웨이브 저장 정보가 없어 복원 생략");
    }
}




}