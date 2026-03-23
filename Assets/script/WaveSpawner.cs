using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public float waveDuration = 10f;
        public float spawnDelay = 1f;
    }
    
    // 기존 변수 + 아래 변수 추가
private bool restoredFromSave = false; // ✅ 복원된 상태인지 확인하는 변수


    [Header("웨이브 설정")]
    public List<Wave> waves = new List<Wave>();
    private int currentWaveIndex = 0;

    [Header("스폰 위치 설정")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("보스 설정")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public string bossName = "보스";
    public float bossSpawnDelay = 5f;                      // 보스 생성 전 대기 시간

    [Header("보스 사망 후 씬 전환")]
    public float sceneTransitionDelay = 5f;                // 보스 사망 후 씬 전환 대기 시간
    public string nextSceneName = "GameOverScene";         // 씬 전환 대상

    [Header("UI 설정")]
    public TextMeshProUGUI waveTimerText;
    public TextMeshProUGUI waveNameText;

    private bool bossSpawned = false;

    // ✅ 오브젝트 풀 저장소
    private Dictionary<GameObject, List<GameObject>> objectPools = new();

    // ✅ 진행 상태 저장용
    private float remainingTime = 0f;
    private float spawnTimer = 0f;

    // ✅ 외부에서 접근 가능한 속성
    public int CurrentWaveIndex => currentWaveIndex;
    public float RemainingTime => remainingTime;
    public float SpawnTimer => spawnTimer;

    [Header("씬 전환 감지 영역 (사각형 박스)")]
public Vector3 areaCenter = Vector3.zero;              // 영역 중심 (World 좌표)
public Vector3 areaSize = new Vector3(5f, 5f, 5f);     // 영역 크기 (가로, 세로, 깊이)
public LayerMask playerLayer;                         // Player 감지용 레이어

[Header("씬 전환 감지 오브젝트")]
public GameObject sceneTransitionArea;


private void Start()
{
    if (PlayerPrefs.GetInt("ReturnedFromBScene", 0) == 1)
    {
        PlayerPrefs.SetInt("ReturnedFromBScene", 0);  // 재진입 방지 초기화

        var waveSpawner = FindObjectOfType<WaveSpawner>();
        var player = FindObjectOfType<PlayerHealthSystem>();

        if (SceneStateManager.Instance != null && waveSpawner != null)
        {
            Debug.Log("🔁 복원 시도 시작!");

            // ✅ 플레이어 상태 복원
            if (player != null)
            {
                PlayerHealthSaveManager.Instance.RestorePlayerState(player);
            }
            else
            {
                Debug.LogWarning("⚠️ [WaveSpawner] 플레이어를 찾을 수 없습니다. 체력 복원 생략.");
            }

            // ✅ 몬스터 및 오브젝트 상태 복원
            SceneStateManager.Instance.RestoreAllStates();

            // ✅ 웨이브 스폰 상태 복원 (🌟 여기가 중요!)
            waveSpawner.RestoreWaveSpawnerState();
        }
        else
        {
            Debug.LogWarning("⚠️ WaveSpawner 또는 SceneStateManager가 존재하지 않아 복원 실패");
        }
    }
    else
    {
        Debug.Log("ℹ️ 복원 플래그가 설정되지 않음 (ReturnedFromBScene != 1)");
    }

    // ✅ 복원 여부와 관계없이 웨이브 시작 준비
    StartCoroutine(WaitAndStart());
}



private IEnumerator WaitAndStart()
{
    yield return null; // ✅ 한 프레임 대기 → SetWaveState가 먼저 실행될 기회를 줌

    if (!restoredFromSave)
    {
        StartCoroutine(StartWaveSequence());
    }
}

public void PreloadPrefabs()
{
    foreach (var wave in waves)
    {
        if (wave.enemyPrefab != null && !objectPools.ContainsKey(wave.enemyPrefab))
        {
            objectPools[wave.enemyPrefab] = new List<GameObject>();
        }
    }

    if (bossPrefab != null && !objectPools.ContainsKey(bossPrefab))
    {
        objectPools[bossPrefab] = new List<GameObject>();
    }
}



    private void UpdateWaveNameText(int index, string waveName)
    {
        if (waveNameText != null)
        {
            waveNameText.text = $"{index + 1}. {waveName}";
        }
    }

    private IEnumerator StartWaveSequence()
    {
        while (currentWaveIndex < waves.Count)
        {
            Wave wave = waves[currentWaveIndex];

            UpdateWaveNameText(currentWaveIndex, wave.waveName);

            remainingTime = wave.waveDuration;
            spawnTimer = 0f;

            StartCoroutine(SpawnWaveEnemies(wave));
            yield return StartCoroutine(HandleWaveCountdown(wave.waveDuration));

            currentWaveIndex++;
        }

        StartCoroutine(SpawnBoss());
    }

    private IEnumerator SpawnWaveEnemies(Wave wave)
    {
        float elapsed = 0f;
        while (elapsed < wave.waveDuration)
        {
            SpawnPooledEnemy(wave.enemyPrefab);
            yield return new WaitForSeconds(wave.spawnDelay);

            spawnTimer += wave.spawnDelay;
            elapsed += wave.spawnDelay;
        }
    }

private void SpawnPooledEnemy(GameObject prefab)
{
    if (spawnPoints.Count == 0 || prefab == null) return;

    int index = Random.Range(0, spawnPoints.Count);
    Transform spawnPoint = spawnPoints[index];

    GameObject enemy = GetPooledObject(prefab, spawnPoint.position, Quaternion.identity);
    if (enemy != null)
    {
        AssignUniqueIDIfNeeded(enemy);
        enemy.SetActive(true);
    }
}



    private GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new List<GameObject>();
        }

        foreach (GameObject obj in objectPools[prefab])
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.SetPositionAndRotation(position, rotation);
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab, position, rotation);
        objectPools[prefab].Add(newObj);
        return newObj;
    }

    private IEnumerator HandleWaveCountdown(float totalTime)
    {
        remainingTime = totalTime;

        while (remainingTime > 0f)
        {
            UpdateWaveTimerText(remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        UpdateWaveTimerText(0f);
    }

    private void UpdateWaveTimerText(float time)
    {
        if (waveTimerText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            waveTimerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

private IEnumerator SpawnBoss()
{
    if (bossSpawned || bossPrefab == null || bossSpawnPoint == null)
        yield break;

    bossSpawned = true;

    if (waveNameText != null)
    {
        waveNameText.text = $"보스: {bossName}";
    }

    // ✅ DiceSpawner 차단
    DiceSpawner[] diceSpawners = FindObjectsOfType<DiceSpawner>();
    foreach (var spawner in diceSpawners)
    {
        spawner.DisableAllSpawnedDice();
        spawner.enabled = false;
    }

    DiceResultTracker.Instance.ResetCounts();

    GameObject boss = GetPooledObject(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
    boss.SetActive(true);

    Debug.Log("👑 보스 등장!");

    // ✅ 보스가 죽을 때까지 대기
    yield return new WaitUntil(() => boss == null || !boss.activeInHierarchy);

    Debug.Log("✅ 보스 클리어, 씬 전환 감지 영역 활성화 준비");

    // ✅ 전환 딜레이 대기
    yield return new WaitForSeconds(sceneTransitionDelay);

    // ✅ 감지 영역 활성화
    Debug.Log("🟢 씬 전환 감지 영역 활성화됨");
    StartCoroutine(WaitForPlayerAndTransition());
}



 // WaveSpawner.cs 내
// WaveSpawner.cs 내
public void SetWaveState(int index, float remainingTime, float spawnTimer)
{
    currentWaveIndex = index;
    this.remainingTime = remainingTime;
    this.spawnTimer = spawnTimer;
    restoredFromSave = true; // ✅ 복원 여부 표시

    Debug.Log($"🔄 [WaveSpawner] 웨이브 복원: index={index}, remainingTime={remainingTime}, spawnTimer={spawnTimer}");

    StopAllCoroutines();
    StartCoroutine(ResumeWave(index, remainingTime, spawnTimer));
}


private IEnumerator ResumeWave(int index, float remainingTime, float spawnTimer)
{
    UpdateWaveNameText(index, waves[index].waveName);

    float spawnElapsed = spawnTimer;

    while (remainingTime > 0f)  // ✅ remainingTime 기준으로 수정
    {
        if (spawnElapsed >= waves[index].spawnDelay)
        {
            SpawnPooledEnemy(waves[index].enemyPrefab);
            spawnElapsed = 0f;
        }

        UpdateWaveTimerText(remainingTime);  // ✅ remainingTime으로 표시

        yield return new WaitForSeconds(1f);
        remainingTime -= 1f;
        spawnElapsed += 1f;
    }

    UpdateWaveTimerText(0f);

    currentWaveIndex++;  // ✅ 다음 웨이브로 넘어가도록 추가

    // ✅ 복원 이후에도 다음 웨이브가 자연스럽게 이어지게 함
    StartCoroutine(StartWaveSequence());
}




private void AssignUniqueIDIfNeeded(GameObject obj)
{
    var assigner = obj.GetComponent<UniqueIDAssigner>();
    if (assigner != null && string.IsNullOrEmpty(assigner.uniqueID))
    {
        assigner.uniqueID = System.Guid.NewGuid().ToString();
    }
}


public GameObject GetPrefabByTypeID(int monsterTypeID)
{
    foreach (var pair in objectPools)
    {
        var typeHolder = pair.Key.GetComponent<MonsterTypeHolder>();
        if (typeHolder != null && typeHolder.monsterTypeID == monsterTypeID)
        {
            return pair.Key;
        }
    }
    return null;
}



public GameObject GetPrefabByName(string prefabName)
{
    foreach (var pair in objectPools)
    {
        var holder = pair.Key.GetComponent<PrefabNameHolder>();
        if (holder != null && holder.prefabName == prefabName)
            return pair.Key;
    }
    return null;
}

public GameObject InstantiateAndRegister(GameObject prefab, Vector3 pos, Quaternion rot)
{
    var obj = Instantiate(prefab, pos, rot);
    if (!objectPools.ContainsKey(prefab))
    {
        objectPools[prefab] = new List<GameObject>();
    }
    objectPools[prefab].Add(obj);
    return obj;
}

public GameObject GetPooledObjectByID(string uniqueID)
{
    foreach (var pool in objectPools.Values)
    {
        foreach (var obj in pool)
        {
            var assigner = obj.GetComponent<UniqueIDAssigner>();
            if (assigner != null && assigner.uniqueID == uniqueID)
            {
                return obj;
            }
        }
    }
    return null; // 못 찾으면 null 반환
}

public void ClearSpawnerData()
{
    Debug.Log("🚫 [WaveSpawner] 웨이브 정보 초기화");

    currentWaveIndex = 0;
    bossSpawned = false;
    remainingTime = 0f;
    spawnTimer = 0f;
    restoredFromSave = false;

    if (objectPools != null)
    {
        objectPools.Clear();  // ✅ 모든 오브젝트 풀 초기화
    }

    StopAllCoroutines();      // ✅ 웨이브 코루틴도 중단

    // 필요한 경우 UI 텍스트 초기화
    if (waveTimerText != null) waveTimerText.text = "";
    if (waveNameText != null) waveNameText.text = "";
}

public static WaveSpawner Instance;

private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Destroy(gameObject);  // 중복 방지
    }
}

private IEnumerator WaitForPlayerAndTransition()
{
    bool playerInArea = false;

    Debug.Log("🔎 플레이어가 감지 영역에 들어오기를 기다립니다.");

    while (!playerInArea)
    {
        // ✅ 영역 안의 모든 Collider 확인
        Collider[] hits = Physics.OverlapBox(areaCenter, areaSize / 2f, Quaternion.identity, playerLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerInArea = true;
                break;
            }
        }

        yield return null; // 매 프레임 검사
    }

    // ✅ 영역에 들어오면 씬 전환
    Debug.Log($"▶ 플레이어가 영역에 진입하여 {nextSceneName} 씬으로 이동!");

    // ✅ 저장된 데이터 초기화
    SceneStateManager.Instance.ClearAllSavedStates();
    PlayerPrefs.DeleteAll(); // 필요한 경우

    SceneManager.LoadScene(nextSceneName);
}


public void StoreWaveState()
{
    PlayerPrefs.SetInt("WaveIndex", currentWaveIndex);
    PlayerPrefs.SetFloat("RemainingTime", remainingTime);
    PlayerPrefs.SetFloat("SpawnTimer", spawnTimer);
    PlayerPrefs.Save();

    Debug.Log($"💾 [WaveSpawner] 웨이브 저장 완료: WaveIndex={currentWaveIndex}, RemainingTime={remainingTime}, SpawnTimer={spawnTimer}");
}

public void RestoreWaveState()
{
    if (PlayerPrefs.HasKey("WaveIndex"))
    {
        int index = PlayerPrefs.GetInt("WaveIndex");
        float remTime = PlayerPrefs.GetFloat("RemainingTime");
        float spwnTimer = PlayerPrefs.GetFloat("SpawnTimer");

        SetWaveState(index, remTime, spwnTimer);
        Debug.Log($"🔁 [WaveSpawner] 웨이브 복원: WaveIndex={index}, RemainingTime={remTime}, SpawnTimer={spwnTimer}");
    }
    else
    {
        Debug.Log("⚠️ [WaveSpawner] 저장된 웨이브 정보가 없어 복원하지 않음.");
    }
}

public void RestoreWaveSpawnerState()
{
    if (PlayerPrefs.HasKey("WaveIndex"))
    {
        int index = PlayerPrefs.GetInt("WaveIndex");
        float remTime = PlayerPrefs.GetFloat("RemainingTime");
        float spwnTimer = PlayerPrefs.GetFloat("SpawnTimer");

        SetWaveState(index, remTime, spwnTimer);
        restoredFromSave = true;  // ✅ 여기 필수!
        Debug.Log($"🔁 [WaveSpawner] 웨이브 복원 완료: WaveIndex={index}, RemainingTime={remTime}, SpawnTimer={spwnTimer}");
    }
    else
    {
        Debug.Log("⚠️ [WaveSpawner] 저장된 웨이브 정보가 없어 복원하지 않음.");
    }
}

private void OnDrawGizmosSelected()
{
    Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // 초록 반투명
    Gizmos.DrawCube(areaCenter, areaSize);
    Gizmos.color = Color.green;
    Gizmos.DrawWireCube(areaCenter, areaSize);
}


}
