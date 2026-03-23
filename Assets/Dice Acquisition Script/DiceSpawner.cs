using UnityEngine;

public class DiceSpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public GameObject dicePrefab;

    [Header("스폰 위치 범위")]
    public Vector2 spawnAreaMin = new Vector2(-5f, -5f);
    public Vector2 spawnAreaMax = new Vector2(5f, 5f);
    public float fixedZPosition = 1f; // ✅ z축 고정값

    [Header("스폰 간격")]
    public float spawnInterval = 5f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnDice();
            timer = 0f;
        }
    }

    private void SpawnDice()
    {
        if (dicePrefab == null) return;

        Vector2 randomXY = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        Vector3 spawnPos = new Vector3(randomXY.x, randomXY.y, fixedZPosition);
        Instantiate(dicePrefab, spawnPos, Quaternion.identity);
    }

    // ✅ 씬 뷰에서 스폰 영역 표시
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f); // 노란 반투명 박스

        Vector3 center = new Vector3(
            (spawnAreaMin.x + spawnAreaMax.x) / 2f,
            (spawnAreaMin.y + spawnAreaMax.y) / 2f,
            fixedZPosition
        );

        Vector3 size = new Vector3(
            spawnAreaMax.x - spawnAreaMin.x,
            spawnAreaMax.y - spawnAreaMin.y,
            0.2f // z 두께는 시각용이므로 얇게
        );

        Gizmos.DrawCube(center, size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }

public void DisableAllSpawnedDice()
{
    GameObject[] allDice = GameObject.FindGameObjectsWithTag("Dice");
    foreach (GameObject dice in allDice)
    {
        if (dice.activeInHierarchy)
        {
            dice.SetActive(false);
        }
    }
}


}
