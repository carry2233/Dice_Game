using UnityEngine;

public class MoveAndRotate2D : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float sprintSpeed = 12f;
    public float mouseRightClickSpeed = 2.5f;
    public float additionalRotationAngle = 45f;

    [Header("회전 속도 설정")]
    public float rotationSpeed = 720f; // 초당 회전 속도 (도 단위)

    [Header("키 설정")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode sprintKey = KeyCode.LeftControl;

    [Header("외부 오브젝트")]
    public GameObject aObject; // ✅ a 오브젝트를 인스펙터에 할당

    [HideInInspector]
    public bool isInputBlocked = false;

    [HideInInspector]
    public bool isRotationBlocked = false;

    private void Update()
    {
        if (isInputBlocked) return;

        Vector2 direction = Vector2.zero;

        // 방향 입력
        if (Input.GetKey(upKey)) direction += Vector2.up;
        if (Input.GetKey(downKey)) direction += Vector2.down;
        if (Input.GetKey(leftKey)) direction += Vector2.left;
        if (Input.GetKey(rightKey)) direction += Vector2.right;

        if (direction != Vector2.zero)
        {
            float currentMoveSpeed;

            if (Input.GetMouseButton(1))
            {
                // ✅ 우클릭 중이고 a 오브젝트가 씬에서 활성화 상태일 때 속도 절반
                if (aObject != null && aObject.activeInHierarchy)
                {
                    currentMoveSpeed = mouseRightClickSpeed * 0.5f;
                }
                else
                {
                    currentMoveSpeed = mouseRightClickSpeed;
                }
            }
            else if (Input.GetKey(sprintKey))
            {
                currentMoveSpeed = sprintSpeed;
            }
            else if (Input.GetKey(runKey))
            {
                currentMoveSpeed = runSpeed;
            }
            else
            {
                currentMoveSpeed = moveSpeed;
            }

            transform.Translate(direction.normalized * currentMoveSpeed * Time.deltaTime, Space.World);

            // 마우스 회전 안할 때만 방향 회전
            if (!Input.GetMouseButton(1))
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // 우클릭 중 마우스 방향 회전
        if (Input.GetMouseButton(1) && !isRotationBlocked)
        {
            RotateTowardsMouseWithOffset();
        }
    }

    private void RotateTowardsMouseWithOffset()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 directionToMouse = (worldMousePosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        targetAngle += additionalRotationAngle;

        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
