using UnityEngine;

[System.Serializable]
public class ObjectStateData
{
    public string uniqueID;
    public int monsterTypeID; // ✅ 추가
    public string prefabName;

    public Vector3 position;
    public Quaternion rotation;

    public float currentHealth;

    public float knockbackForce;
    public float knockbackSpeedMultiplier;
    public float knockbackDuration;
    public float knockbackDelay;

    public float delayBeforeCharge;
    public float chargeDuration;
}
