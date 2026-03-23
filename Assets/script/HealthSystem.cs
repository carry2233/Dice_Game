using UnityEngine;
using System.Collections.Generic;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Resistance Settings")]
    public float resistanceValue = 10f;

    [Header("Prefab Settings")]
    public GameObject spawnOnDamagePrefab;
    public float spawnRange = 2f;

    [Header("Damage Text Settings")]
    public GameObject damageTextPrefab;

    [Header("Charge Damage Cooldown")]
public float chargeDamageCooldown = 0.5f;  // 기본 쿨타임
private Dictionary<GameObject, float> chargeDamageTimers = new();

    // ✅ 데미지 주기 처리용 타이머 관리
    private Dictionary<GameObject, float> laserTimers = new();

    private void Start()
    {
        if (currentHealth <= 0)
        {
            DisableObject();
        }
    }

private void Update()
{
    List<GameObject> keys = new List<GameObject>(laserTimers.Keys);
    foreach (var laser in keys)
    {
        laserTimers[laser] -= Time.deltaTime;
        if (laserTimers[laser] <= 0f)
            laserTimers[laser] = 0f;
    }

    List<GameObject> chargeKeys = new List<GameObject>(chargeDamageTimers.Keys);
    foreach (var attacker in chargeKeys)
    {
        chargeDamageTimers[attacker] -= Time.deltaTime;
        if (chargeDamageTimers[attacker] <= 0f)
            chargeDamageTimers[attacker] = 0f;
    }
}

    // ✅ 외부에서 주기적 데미지를 요청할 때 호출
    public bool TryApplyLaserDamage(GameObject laser, float attack, float penetration, float resistance, float interval)
    {
        if (penetration < resistance)
            return false;

        if (!laserTimers.ContainsKey(laser) || laserTimers[laser] <= 0f)
        {
            ApplyFullDamage(attack);
            laserTimers[laser] = interval;
            return true;
        }

        return false;
    }

    public void ApplyFullDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[HealthSystem] 체력 감소: {damage} → 남은 체력: {currentHealth}");

        if (spawnOnDamagePrefab != null)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spawnRange;
            Vector3 spawnPosition = transform.position + randomOffset;
            SpawnOnDamagePoolManager.Instance?.SpawnFromPool(spawnOnDamagePrefab, spawnPosition, Quaternion.identity);
        }

        if (damageTextPrefab != null)
        {
            DamageTextPoolManager.Instance?.ShowDamageText(transform, damage);
        }

        if (currentHealth <= 0)
        {
            DisableObject();
        }
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
        currentHealth = maxHealth;
    }
}
