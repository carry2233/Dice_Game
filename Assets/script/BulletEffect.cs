using UnityEngine;

public class BulletEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public GameObject deactivateEffectPrefab;

    private BulletKnockback knockback;
    private DamageHandler damageHandler;

    private void Awake()
    {
        knockback = GetComponent<BulletKnockback>();
        damageHandler = GetComponent<DamageHandler>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 넉백 적용
            if (knockback != null)
            {
                knockback.ApplyKnockback(collision);
            }

            // 데미지 처리
            if (damageHandler != null)
            {
                damageHandler.ReducePenetration(0); // 관통력 차감
            }

            // 이펙트 생성은 OnDisable에서 처리됨
        }
    }

    private void OnDisable()
    {
        // 비활성화 직전에 이펙트 생성
        if (deactivateEffectPrefab != null)
        {
            Instantiate(deactivateEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}
