// ✅ StateSaver.cs
using UnityEngine;

public class StateSaver : MonoBehaviour
{
public ObjectStateData SaveState()
{
    var data = new ObjectStateData();

    data.uniqueID = GetComponent<UniqueIDAssigner>()?.uniqueID;

    // ✅ monsterTypeID 저장
    var typeHolder = GetComponent<MonsterTypeHolder>();
    data.monsterTypeID = typeHolder != null ? typeHolder.monsterTypeID : -1;

    var nameHolder = GetComponent<PrefabNameHolder>();
    data.prefabName = nameHolder != null ? nameHolder.prefabName : "";

    data.position = transform.position;
    data.rotation = transform.rotation;

    var health = GetComponent<HealthSystem>();
    if (health != null)
    {
        data.currentHealth = health.currentHealth;
    }

    var knockback = GetComponent<KnockbackHandler>();
    if (knockback != null)
    {
        data.knockbackForce = knockback.knockbackForce;
        data.knockbackSpeedMultiplier = knockback.knockbackSpeedMultiplier;
        data.knockbackDuration = knockback.knockbackDuration;
        data.knockbackDelay = knockback.knockbackDelay;
    }

    var ai = GetComponent<TimedChargeAI>();
    if (ai != null)
    {
        data.delayBeforeCharge = ai.delayBeforeCharge;
        data.chargeDuration = ai.chargeDuration;
    }

    return data;
}

 public void LoadState(ObjectStateData data)
{
    transform.SetPositionAndRotation(data.position, data.rotation);

    var health = GetComponent<HealthSystem>();
    if (health != null)
    {
        health.currentHealth = data.currentHealth;
        Debug.Log($"🩸 {name} 체력 복원됨 → {data.currentHealth}");
    }

    var knockback = GetComponent<KnockbackHandler>();
    if (knockback != null)
    {
        knockback.knockbackForce = data.knockbackForce;
        knockback.knockbackSpeedMultiplier = data.knockbackSpeedMultiplier;
        knockback.knockbackDuration = data.knockbackDuration;
        knockback.knockbackDelay = data.knockbackDelay;

        Debug.Log($"💥 {name} 넉백 수치 복원됨");
    }

    var ai = GetComponent<TimedChargeAI>();
    if (ai != null)
    {
        ai.delayBeforeCharge = data.delayBeforeCharge;
        ai.chargeDuration = data.chargeDuration;

        Debug.Log($"🤖 {name} AI 복원됨");
    }
}

}