using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [Header("발사 설정")]
    public Transform firePoint;
    public GameObject laserVisualPrefab;
    public Vector3 laserOffset = new Vector3(6f, 0f, 0f); // 기본값 6
    public Vector3 laserRotation = new Vector3(0f, 90f, 0f);
    public Vector3 laserScale = Vector3.one;
    public KeyCode toggleModeKey = KeyCode.F;

    [Header("레이어 설정")]
    public LayerMask targetLayer;

    private GameObject laserVisualInstance;
    private LaserVisual laserVisualScript;
    private DamageHandler damageHandler;

    private enum FireMode { HoldToFire, ToggleFire }
    private FireMode currentFireMode = FireMode.HoldToFire;
    private bool isLaserToggled = false;

    private float initialLaserOffsetX;

    private void Start()
    {
        if (laserVisualPrefab != null)
        {
            laserVisualInstance = Instantiate(laserVisualPrefab);
            laserVisualInstance.SetActive(false);
            laserVisualScript = laserVisualInstance.GetComponent<LaserVisual>();
        }

        damageHandler = GetComponent<DamageHandler>();
        if (damageHandler == null)
        {
            Debug.LogError("[LaserShooter] DamageHandler가 없습니다.");
        }

        initialLaserOffsetX = laserOffset.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleModeKey))
        {
            currentFireMode = currentFireMode == FireMode.HoldToFire ? FireMode.ToggleFire : FireMode.HoldToFire;
            isLaserToggled = false;
            DisableLaserVisual();
        }

        bool isRightClickHeld = Input.GetMouseButton(1);
        bool isLeftClick = Input.GetMouseButton(0);
        bool isLeftClickDown = Input.GetMouseButtonDown(0);

        if (currentFireMode == FireMode.HoldToFire)
        {
            if (isRightClickHeld && isLeftClick)
            {
                EnableLaserVisual();
                UpdateLaserVisualTransform();
            }
            else
            {
                DisableLaserVisual();
                isLaserToggled = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                DisableLaserVisual();
            }
        }
        else if (currentFireMode == FireMode.ToggleFire)
        {
            if (isLeftClickDown)
            {
                isLaserToggled = !isLaserToggled;
                if (isLaserToggled)
                    EnableLaserVisual();
                else
                    DisableLaserVisual();
            }

            if (isLaserToggled)
            {
                UpdateLaserVisualTransform();
            }
        }
    }

    private void UpdateLaserVisualTransform()
    {
        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.forward.normalized;

        float maxLaserLength = initialLaserOffsetX * 2f;
        float adjustedLaserLength = maxLaserLength;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, maxLaserLength, targetLayer))
        {
            HealthSystem health = hit.collider.GetComponentInParent<HealthSystem>();
            if (health != null)
            {
                float resistance = health.resistanceValue;
                float penetration = damageHandler.penetrationValue;

                if (resistance >= penetration)
                {
                    adjustedLaserLength = hit.distance;
                }
            }
        }

        // 막혔을 때만 조정, 막히지 않으면 초기값 유지
        laserOffset.x = adjustedLaserLength / 2f;

        Vector3 finalOffset =
            firePoint.right * laserOffset.x +
            firePoint.up * laserOffset.y +
            firePoint.forward * laserOffset.z;

        Vector3 offsetPosition = origin + direction * 0.5f + finalOffset;

        laserVisualInstance.transform.position = offsetPosition;
        laserVisualInstance.transform.rotation = Quaternion.Euler(laserRotation.x, laserRotation.y, firePoint.rotation.eulerAngles.z);
        Vector3 newScale = laserScale;
        newScale.x = laserOffset.x * 2f;
        laserVisualInstance.transform.localScale = newScale;
    }

    private void EnableLaserVisual()
    {
        if (laserVisualInstance != null && !laserVisualInstance.activeSelf)
            laserVisualInstance.SetActive(true);
    }

    private void DisableLaserVisual()
    {
        if (laserVisualInstance != null && laserVisualInstance.activeSelf)
            laserVisualInstance.SetActive(false);
    }
}
