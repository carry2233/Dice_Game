using UnityEngine;
using System.Collections;

public class KnockbackHandler : MonoBehaviour
{
    private bool isKnockedBack = false;
    private float knockbackEndTime;
    private Rigidbody rb;
    private Monster monster;

    // ✅ StateSaver가 접근할 수 있도록 public 필드로 정의
    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackSpeedMultiplier = 1f;
    public float knockbackDuration = 0.2f;
    public float knockbackDelay = 0f;

    private void Start()
    {
        monster = GetComponent<Monster>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("[KnockbackHandler] Rigidbody 컴포넌트가 없습니다.");
    }

    private void Update()
    {
        if (isKnockedBack && Time.time >= knockbackEndTime)
        {
            StopKnockback();
        }
    }

    private void StopKnockback()
    {
        isKnockedBack = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        if (monster != null)
        {
            monster.SetKnockbackState(false);
        }
    }

    private void OnDisable()
    {
        StopKnockback();
    }

    private void OnEnable()
    {
        isKnockedBack = false;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 외부에서 넉백 적용 시 호출
    /// 전달받은 넉백 수치를 내부 변수에 저장 → StateSaver에 노출 가능
    /// </summary>
    public void ApplyKnockback(Vector3 direction, float force, float speedMultiplier, float duration, float delay)
    {
        // ✅ StateSaver를 위한 저장
        knockbackForce = force;
        knockbackSpeedMultiplier = speedMultiplier;
        knockbackDuration = duration;
        knockbackDelay = delay;

        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(ApplyKnockbackCoroutine(direction, force, speedMultiplier, duration, delay));
    }

    private IEnumerator ApplyKnockbackCoroutine(Vector3 direction, float force, float speedMultiplier, float duration, float delay)
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        if (!gameObject.activeInHierarchy || rb == null) yield break;

        Vector3 knockbackDir = direction.normalized;

        // ✅ 방향 벡터가 유효하지 않으면 넉백 없이 정지 상태로 전환
        bool shouldOnlyStun = knockbackDir == Vector3.zero ||
                              float.IsNaN(knockbackDir.x) || float.IsNaN(knockbackDir.y) || float.IsNaN(knockbackDir.z);

        isKnockedBack = true;
        knockbackEndTime = Time.time + duration;

        if (monster != null)
        {
            monster.SetKnockbackState(true);
        }

        if (shouldOnlyStun)
        {
            Debug.LogWarning("[KnockbackHandler] 방향이 유효하지 않아 이동 없이 멈춤 효과만 적용됨.");
            yield break;
        }

        rb.velocity = knockbackDir * (force / duration) * speedMultiplier;
    }
}
