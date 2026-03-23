using UnityEngine;

public class Monster : ChaseControllable
{
    public float followSpeed = 5f;
    public float rotationSnapAngle = 45f;
    public float rotationSpeed = 360f;

    private bool isKnockedBack = false;
    private float originalFollowSpeed;

    private void Start()
    {
        originalFollowSpeed = followSpeed;
    }

    private void Update()
    {
        bool limiterActive = GameObject.FindObjectOfType<ChaseRangeLimiter>()?.enabled ?? false;

        if (target == null || Vector3.Distance(transform.position, target.position) > maxChaseDistance)
        {
            if (limiterActive)
                FindNearestPlayerInRadius(transform.position, maxChaseDistance);
            else
                FindNearestPlayerAnywhere();

            if (target == null) return;
        }

        if (!isChaseEnabled || isKnockedBack) return;

        MoveTowardsTarget();
    }

    public void SetKnockbackState(bool state)
    {
        isKnockedBack = state;
    }

    private void MoveTowardsTarget()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 snappedDirection = SnapToAngle(directionToTarget, rotationSnapAngle);
        transform.position += snappedDirection * followSpeed * Time.deltaTime;

        float angle = Mathf.Atan2(snappedDirection.y, snappedDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    private Vector3 SnapToAngle(Vector3 direction, float snapAngle)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / snapAngle) * snapAngle;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
    }

    public void ApplySpeedModifier(float multiplier)
    {
        followSpeed = originalFollowSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        followSpeed = originalFollowSpeed;
    }
}
