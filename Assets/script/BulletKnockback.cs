using UnityEngine;

public class BulletKnockback : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackSpeedMultiplier = 1f;
    public float knockbackDuration = 0.2f;
    public float knockbackDelay = 0f;

    private void OnTriggerEnter(Collider other) // ✅ 충돌 감지 추가
    {
        ApplyKnockback(other);
    }

    public void ApplyKnockback(Collider other)
    {
        KnockbackHandler knockbackHandler = other.GetComponent<KnockbackHandler>();
        if (knockbackHandler != null)
        {
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;

            knockbackHandler.ApplyKnockback(
                knockbackDirection,
                knockbackForce,
                knockbackSpeedMultiplier,
                knockbackDuration,
                knockbackDelay
            );
        }
    }
}
