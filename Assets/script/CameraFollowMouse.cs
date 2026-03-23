using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{
    [Header("Camera Settings")]
    public float followSpeed = 5f;
    public Transform player;

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("Movement Settings")]
    public float baseRadius = 5f;
    public float zoomMultiplier = 1f;
    public bool useCircularLimit = true;

    [Header("Mouse Influence Settings")]
    public float movementFactor = 2f;
    public float maxMouseDistance = 0.5f;

    [Header("Key Settings")]
    public KeyCode toggleFollowKey = KeyCode.Space;

    private float currentRadius;
    private bool isMouseFollowEnabled = true;
    private float currentZoom;

    private Vector3 lastPlayerPosition;

    private Camera mainCam;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("플레이어 Transform이 설정되지 않았습니다.");
            enabled = false;
            return;
        }

        mainCam = Camera.main;

        if (mainCam == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다.");
            enabled = false;
            return;
        }

        currentZoom = mainCam.orthographicSize;
        UpdateRadius();
        lastPlayerPosition = player.position;
    }

    private void Update()
    {
        HandleToggleFollow();
        HandleZoom();
    }

    private void LateUpdate()
    {
        if (isMouseFollowEnabled && Input.GetMouseButton(1))
        {
            FollowMouse();
        }
        else
        {
            FollowPlayer();
        }

        lastPlayerPosition = player.position;
    }

    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            currentZoom = Mathf.Clamp(currentZoom - scrollInput * zoomSpeed, minZoom, maxZoom);
            mainCam.orthographicSize = currentZoom;
            UpdateRadius();
        }
    }

    private void UpdateRadius()
    {
        currentRadius = baseRadius * (1 + (currentZoom - minZoom) * zoomMultiplier / (maxZoom - minZoom));
    }

    private void FollowPlayer()
    {
        MoveCamera(lastPlayerPosition); // 부드러운 이동
    }

    private void FollowMouse()
    {
        if (mainCam == null || player == null) return;

        Vector3 mouseViewportPos = mainCam.ScreenToViewportPoint(Input.mousePosition);
        Vector2 offsetFromCenter = new Vector2(mouseViewportPos.x - 0.5f, mouseViewportPos.y - 0.5f);

        float distance = offsetFromCenter.magnitude;
        float movementStrength = Mathf.Clamp(distance / maxMouseDistance, 0f, 1f) * movementFactor;

        Vector3 targetFocusPoint = player.position + new Vector3(offsetFromCenter.x, offsetFromCenter.y, 0) * movementStrength * currentRadius;

        MoveCamera(targetFocusPoint);
    }

    private void MoveCamera(Vector3 targetPosition)
    {
        if (useCircularLimit)
        {
            Vector3 direction = (targetPosition - player.position).normalized;
            float distance = Vector3.Distance(targetPosition, player.position);
            float clampedDistance = Mathf.Min(distance, currentRadius);
            targetPosition = player.position + direction * clampedDistance;
        }

        targetPosition.z = transform.position.z; // Z 위치 유지
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void HandleToggleFollow()
    {
        if (Input.GetKeyDown(toggleFollowKey))
        {
            isMouseFollowEnabled = !isMouseFollowEnabled;
            Debug.Log($"Mouse Follow Enabled: {isMouseFollowEnabled}");
        }
    }

    private void OnDrawGizmos()
    {
        if (player != null && useCircularLimit)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, currentRadius);
        }
    }
}
