using System.Collections;
using UnityEngine;

public class AreaDamageDealer : MonoBehaviour
{
    [Header("범위 설정")]
    public float damageRadius = 3f; // 원형 범위
    public float damageInterval = 1f; // 피해 주기

    private DamageHandler damageHandler;
    private Coroutine damageCoroutine;

    private void Awake()
    {
        damageHandler = GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            Debug.LogError("[AreaDamageDealer] DamageHandler 컴포넌트가 필요합니다.");
        }
    }

    private void OnEnable()
    {
        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DealDamagePeriodically());
        }
    }

    private void OnDisable()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private IEnumerator DealDamagePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);
            DealAreaDamage();
        }
    }

    private void DealAreaDamage()
    {
        if (!damageHandler.CanDealDamage()) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider hit in hits)
        {
            HealthSystem target = hit.GetComponent<HealthSystem>();
            if (target != null)
            {
                target.ApplyFullDamage(damageHandler.GetAttackValue());
                Debug.Log($"[AreaDamageDealer] {hit.gameObject.name}에게 {damageHandler.GetAttackValue()} 피해를 줌");
            }
        }
    }

    // ✅ 씬 뷰에서 공격 범위를 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.4f); // 반투명 붉은색
        Gizmos.DrawSphere(transform.position, damageRadius); // 채워진 구 표시

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius); // 테두리 표시
    }
}
