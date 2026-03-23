using UnityEngine;

public class Aiming : MonoBehaviour
{
    [Header("Fixed Offset from Parent")]
    public Vector3 fixedOffset = Vector3.zero;
    public Vector3 fixedOffsetWhenRightClick = Vector3.zero;

    [Header("Rotation Control")]
    public float alternateRotationZ = 45f;

    [Header("Parent Control")]
    public Transform parentObject;

    private bool isRecoiling = false;
    private Vector3 aimingBasePosition;

    private void Start()
    {
        InitializePositionAndRotation();
    }

    private void Update()
    {
        if (parentObject != null)
        {
            aimingBasePosition = parentObject.position + parentObject.rotation * fixedOffsetWhenRightClick;
        }
    }

    private void LateUpdate()
    {
        if (parentObject == null || isRecoiling) return;
        ApplyParentTransform(); // 무조건 적용
    }

    private void InitializePositionAndRotation()
    {
        if (parentObject != null)
        {
            transform.position = parentObject.position + parentObject.rotation * fixedOffset;
            transform.rotation = parentObject.rotation * Quaternion.Euler(0, 0, alternateRotationZ);
        }
    }

    private void ApplyParentTransform()
    {
        Vector3 finalOffset = Input.GetMouseButton(1) ? fixedOffsetWhenRightClick : fixedOffset;
        transform.position = parentObject.position + parentObject.rotation * finalOffset;

        transform.rotation = Input.GetMouseButton(1)
            ? parentObject.rotation
            : parentObject.rotation * Quaternion.Euler(0, 0, alternateRotationZ);
    }

    public void SetRecoilState(bool isRecoiling)
    {
        this.isRecoiling = isRecoiling;
    }

    public Vector3 GetAimingBasePosition()
    {
        return aimingBasePosition;
    }
}
