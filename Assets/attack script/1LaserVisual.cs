using UnityEngine;

public class LaserVisual : MonoBehaviour
{
    [Header("피격 주기 설정")]
    public float damageInterval = 0.2f;
    public LayerMask targetLayer;

    [Header("레이저 스크롤 설정")]
    public float scrollSpeed = 2f;

    private Material material;
    private DamageHandler damageHandler;

    private float damageTimer = 0f;  // ✅ 내부 타이머

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            material = renderer.material;
        }

        damageHandler = GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            Debug.LogError("[LaserVisual] DamageHandler가 연결되지 않았습니다.");
        }
    }

    private void Update()
    {
        // ✅ 텍스처 스크롤 처리
        if (material != null)
        {
            Vector2 offset = material.mainTextureOffset;
            offset.x += scrollSpeed * Time.deltaTime;
            material.mainTextureOffset = offset;
        }

        // ✅ 주기 데미지 처리
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            DealLaserDamage();
            damageTimer = 0f;
        }
    }

    private void DealLaserDamage()
    {
        if (damageHandler == null || !damageHandler.CanDealDamage())
            return;

        Vector3 center = transform.position;
        Vector3 halfExtents = transform.localScale / 2f;
        Quaternion rotation = transform.rotation;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation, targetLayer);
        foreach (Collider hit in hits)
        {
            HealthSystem health = hit.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                bool damaged = health.TryApplyLaserDamage(
                    laser: gameObject,
                    attack: damageHandler.attackValue,
                    penetration: damageHandler.penetrationValue,
                    resistance: health.resistanceValue,
                    interval: damageInterval  // ✅ 여기도 주기 전달!
                );

                if (damaged)
                {
                    Debug.Log($"[LaserVisual] {hit.gameObject.name}에게 {damageHandler.attackValue} 피해를 주었음 (주기 적용)");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, transform.localScale);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, transform.localScale);
    }
}
