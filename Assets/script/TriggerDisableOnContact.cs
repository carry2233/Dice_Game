using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class TriggerDisableOnContact : MonoBehaviour
{
    [Header("비활성화 제외 태그")]
    public string ignoreTag = "IgnoreDisable"; // 이 태그를 가진 오브젝트는 비활성화 제외

    private void Reset()
    {
        // 자동으로 트리거 설정
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 접촉한 오브젝트가 리지드바디 + 트리거 콜라이더 조건을 갖췄는지 확인
        Rigidbody rb = other.attachedRigidbody;
        Collider col = other.GetComponent<Collider>();

        // ✅ 제외 태그 검사
        if (other.CompareTag(ignoreTag))
        {
            Debug.Log($"[TriggerDisableOnContact] {other.name} 은 {ignoreTag} 태그를 가지고 있어 무시됨.");
            return;
        }

        if (rb != null && col != null && col.isTrigger)
        {
            Debug.Log($"[TriggerDisableOnContact] {other.name} 비활성화됨");
            other.gameObject.SetActive(false);
        }
    }
}
