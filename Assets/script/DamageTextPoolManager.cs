using UnityEngine;
using System.Collections.Generic;

public class DamageTextPoolManager : MonoBehaviour
{
    public static DamageTextPoolManager Instance;

    [Header("UI 텍스트 프리팹")]
    public GameObject damageTextPrefab;

    [Header("풀 크기 설정")]
    public int poolSize = 10;

    [Header("텍스트 위치 오프셋")]
    public Vector3 worldOffset = new Vector3(0, 2, 0);

    private Queue<DamageText> pool = new Queue<DamageText>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(damageTextPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj.GetComponent<DamageText>());
        }
    }

    public void ShowDamageText(Transform target, float damage)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("⚠ DamageTextPool이 비어 있습니다!");
            return;
        }

        DamageText dt = pool.Dequeue();
        dt.gameObject.SetActive(true);
        dt.Setup(target, damage, worldOffset);
        pool.Enqueue(dt);
    }
}
