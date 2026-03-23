using UnityEngine;

public class TransformMovementLimiter : MonoBehaviour
{
    [Header("움직임 제한 범위 (월드 좌표 기준)")]
    public Vector3 minBounds = new Vector3(-10f, -5f, 0f);
    public Vector3 maxBounds = new Vector3(10f, 5f, 0f);

    private void LateUpdate()
    {
        Vector3 clampedPos = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
            Mathf.Clamp(transform.position.z, minBounds.z, maxBounds.z)
        );

        transform.position = clampedPos;
    }

    // 씬 뷰에서 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f); // 청록색 반투명
        Vector3 center = (minBounds + maxBounds) * 0.5f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawCube(center, size);
        Gizmos.color = new Color(0f, 1f, 1f, 1f);
        Gizmos.DrawWireCube(center, size);
    }
}
