using UnityEngine;

public class TimedChargeAI : ChaseControllable
{
    public float delayBeforeCharge = 2f;
    public float chargeDuration = 1f;
    public float chargeSpeed = 5f;
    public float rotateSpeed = 360f;

    [HideInInspector]
    public bool isChargeBlocked = false;

    private float timer = 0f;
    private enum State { Waiting, Charging }
    private State currentState = State.Waiting;

    private Vector3 chargeDirection;

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

        if (!isChaseEnabled) return;
        if (isChargeBlocked && currentState == State.Charging)
        {
            currentState = State.Waiting; // 강제 중단
        }

        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.Waiting:
                RotateTowardsTarget();
                chargeDirection = (target.position - transform.position).normalized;

                if (timer >= delayBeforeCharge && !isChargeBlocked)
                {
                    timer = 0f;
                    currentState = State.Charging;
                }
                break;

            case State.Charging:
                transform.position += chargeDirection * chargeSpeed * Time.deltaTime;

                if (timer >= chargeDuration)
                {
                    timer = 0f;
                    currentState = State.Waiting;
                }
                break;
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
    }
}
