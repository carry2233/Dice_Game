using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float penetrationValue = 15f;     // 투사체의 초기 관통력
    public float attackValue = 10f;          // 현재 공격력
    private float currentPenetrationValue;   // 현재 관통력
    private float baseAttackValue;           // 기본 공격력 저장용

    public float chargeDamageCooldown = 0.5f;


    [Header("Damage Control")]
    public bool canDealDamage = true;

    [Header("주기 설정 (LaserVisual이 있을 경우만 적용)")]
    public float damageInterval = 0.2f;      // 주기적 데미지 간격
    private float damageTimer = 0f;

    private void OnEnable()
    {
        currentPenetrationValue = penetrationValue;
        baseAttackValue = attackValue;
        canDealDamage = true;
        damageTimer = 0f;
    }

    public void ReducePenetration(float reductionValue)
    {
        currentPenetrationValue -= reductionValue;
        if (currentPenetrationValue <= 0)
        {
            DisableProjectile();
        }
    }

    private void DisableProjectile()
    {
        gameObject.SetActive(false);
        currentPenetrationValue = penetrationValue;
    }

    public float GetAttackValue()
    {
        return attackValue;
    }

    public bool CanDealDamage()
    {
        return canDealDamage;
    }

    public void DisableDamage()
    {
        canDealDamage = false;
    }

    public void AddAttackBonus(float bonus)
    {
        attackValue += bonus;
    }

    public void ResetAttackValue()
    {
        attackValue = baseAttackValue;
    }

    // ✅ 핵심 수정: LaserVisual이 있을 때만 damageInterval 적용
    private void OnTriggerStay(Collider other)
    {
        if (!canDealDamage) return;

        bool hasLaserVisual = GetComponent<LaserVisual>() != null;  // LaserVisual이 붙어있는지 확인

        if (hasLaserVisual)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageInterval)
            {
                ApplyDamageToTarget(other);
                damageTimer = 0f;  // 타이머 리셋
            }
        }
        else
        {
            ApplyDamageToTarget(other);  // LaserVisual 없으면 매 프레임 바로 데미지
        }
    }

    private void ApplyDamageToTarget(Collider other)
    {
        HealthSystem target = other.GetComponent<HealthSystem>();
        if (target != null)
        {
            target.ApplyFullDamage(attackValue);
            ReducePenetration(target.resistanceValue);

            if (currentPenetrationValue <= 0)
            {
                DisableDamage();
            }
        }
    }
}
