using UnityEngine;

public class Head : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform playerTransform; // 플레이어 Transform
    public Vector3 offset = new Vector3(0, 1, 0); // 플레이어 기준 상대 위치

    [HideInInspector]
    public bool isRotationBlocked = false; // 외부에서 회전 차단용

    private Quaternion lastRotation; // 마지막 회전값 저장용

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("플레이어 Transform이 할당되지 않았습니다. Inspector에서 설정하세요.");
            enabled = false;
            return;
        }

        lastRotation = transform.rotation; // 초기 회전값 저장
    }

    private void LateUpdate()
    {
        if (playerTransform == null) return;

        // 플레이어 회전 기준 offset 위치 계산
        Vector3 rotatedOffset = playerTransform.rotation * offset;
        transform.position = playerTransform.position + rotatedOffset;

        if (isRotationBlocked)
        {
            // ✅ 회전 차단 상태에서는 마지막 회전값 유지 (회전 고정)
            transform.rotation = lastRotation;
        }
        else if (!Input.GetMouseButton(1))
        {
            // 우클릭 아닐 때 → 플레이어 회전 따라감
            transform.rotation = playerTransform.rotation;
            lastRotation = transform.rotation;
        }
        else
        {
            // 우클릭 중일 때 마우스를 향해 회전
            RotateTowardsMouse();
            lastRotation = transform.rotation;
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 directionToMouse = (worldMousePosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
