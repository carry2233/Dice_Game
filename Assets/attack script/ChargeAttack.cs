using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargeAttack : MonoBehaviour
{
    [Header("공격체 (풀링 대상)")]
    public GameObject attackPrefab;
    public Transform targetObject;
    public Vector3 offsetFromTarget = new Vector3(1f, 0f, 0f);
    public float activeTime = 2f;

    [Header("돌진 오브젝트 (풀링 X)")]
    public GameObject chargeTarget;
    public float chargeSpeed = 5f;
    public float holdChargeSpeed = 3f;
    public float chargeDelay = 0.3f;
    public float nonHoldChargeTime = 1f;

    [Header("돌진 제어")]
    public bool isHoldToCharge = false;
    public float rotationFollowSpeed = 360f;
    public bool dealDamageWhileCharging = false;

    [Header("임팩트 이펙트")]
    public GameObject impactEffectObject;
    public Vector3 impactOffset = Vector3.zero;

    [Header("공격력 감소 설정")]
    public DamageHandler damageHandler;
    public float damageReductionValue = 1f;
    public float damageReductionInterval = 0.2f;
    public float minAttackValue = 2f;

    [Header("제어")]
    public MoveAndRotate2D movementScript;
    private SpriteRenderer bodyRenderer;
    private Collider myCollider;

    [Header("부속 오브젝트 제어")]
    public Head headScript1;
    public Head headScript2;

    private bool isCharging = false;
    private Quaternion originalRotation;
    private Coroutine reductionCoroutine;

    private Vector3 currentDirection;
    private PlayerHealthSystem playerHealthSystem;

    [Header("콜라이더 재활성화 설정")]
public float colliderReactivateDelay = 0.5f;  // 예시: 0.5초 후 다시 활성화


    private void Start()
    {
        bodyRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider>();
        if (myCollider != null) myCollider.enabled = false;
        if (impactEffectObject != null) impactEffectObject.SetActive(false);

        playerHealthSystem = FindObjectOfType<PlayerHealthSystem>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0) && !isCharging)
        {
            StartCoroutine(AttackSequence());
        }

        if (isCharging && isHoldToCharge)
        {
            RotateObjectToMouse(chargeTarget);
            RotateObjectToMouse(this.gameObject);
        }
    }

    private IEnumerator AttackSequence()
    {
        isCharging = true;
        originalRotation = transform.rotation;

        if (movementScript != null)
        {
            movementScript.isInputBlocked = true;
            movementScript.isRotationBlocked = true;
        }

        if (headScript1 != null) headScript1.isRotationBlocked = true;
        if (headScript2 != null) headScript2.isRotationBlocked = true;

        SetTransparency(0.3f);
        if (myCollider != null) myCollider.enabled = true;

        if (playerHealthSystem != null)
        {
            playerHealthSystem.SetInvincibleState(true);
        }

        GameObject attackInstance = SpawnOnDamagePoolManager.Instance.SpawnFromPool(
            attackPrefab,
            targetObject.position + (targetObject.rotation * offsetFromTarget),
            targetObject.rotation
        );
        if (attackInstance != null) attackInstance.SetActive(true);

        if (impactEffectObject != null)
        {
            impactEffectObject.transform.position = targetObject.position + (targetObject.rotation * impactOffset);
            impactEffectObject.transform.rotation = targetObject.rotation;
            impactEffectObject.SetActive(true);
        }

        if (damageHandler != null && isHoldToCharge)
        {
            reductionCoroutine = StartCoroutine(DamageReductionLoop());
        }

        Vector3 fixedDirection = Vector3.zero;

        if (!isHoldToCharge)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            fixedDirection = (mouseWorld - chargeTarget.transform.position).normalized;
            currentDirection = fixedDirection;
        }

        if (isHoldToCharge)
        {
            currentDirection = Vector3.right;
            while (Input.GetMouseButton(0))
            {
                PerformChargeStep(holdChargeSpeed);
                yield return null;
            }
        }
        else
        {
            float t = 0f;
            while (t < nonHoldChargeTime)
            {
                PerformChargeStep(chargeSpeed, fixedDirection);
                RotateTowardsDirection(fixedDirection);
                t += Time.deltaTime;
                yield return null;
            }
        }

        yield return new WaitForSeconds(activeTime);

        if (attackInstance != null) attackInstance.SetActive(false);
        if (impactEffectObject != null) impactEffectObject.SetActive(false);
        if (myCollider != null) myCollider.enabled = false;

        if (reductionCoroutine != null)
            StopCoroutine(reductionCoroutine);

        if (damageHandler != null)
            damageHandler.ResetAttackValue();

        SetTransparency(1f);

        if (movementScript != null)
        {
            movementScript.isInputBlocked = false;
            movementScript.isRotationBlocked = false;
        }

        if (headScript1 != null) headScript1.isRotationBlocked = false;
        if (headScript2 != null) headScript2.isRotationBlocked = false;

        if (playerHealthSystem != null)
        {
            playerHealthSystem.SetInvincibleState(false);
        }

        yield return new WaitForSeconds(chargeDelay);
        isCharging = false;
    }

    private void PerformChargeStep(float speed)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector3 targetDir = (mouseWorld - chargeTarget.transform.position).normalized;

        currentDirection = Vector3.RotateTowards(currentDirection, targetDir, rotationFollowSpeed * Mathf.Deg2Rad * Time.deltaTime, 1f);
        chargeTarget.transform.position += currentDirection * speed * Time.deltaTime;

        if (impactEffectObject != null)
        {
            impactEffectObject.transform.position = targetObject.position + (targetObject.rotation * impactOffset);
        }
    }

    private void PerformChargeStep(float speed, Vector3 direction)
    {
        if (chargeTarget != null && direction != Vector3.zero)
        {
            currentDirection = Vector3.RotateTowards(currentDirection, direction, rotationFollowSpeed * Mathf.Deg2Rad * Time.deltaTime, 1f);
            chargeTarget.transform.position += currentDirection * speed * Time.deltaTime;

            if (impactEffectObject != null)
            {
                impactEffectObject.transform.position = targetObject.position + (targetObject.rotation * impactOffset);
            }
        }
    }

    private void RotateObjectToMouse(GameObject obj)
    {
        if (obj == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 dir = mousePos - obj.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        obj.transform.rotation = Quaternion.RotateTowards(obj.transform.rotation, targetRot, rotationFollowSpeed * Time.deltaTime);
    }

    private void RotateTowardsDirection(Vector3 dir)
    {
        if (chargeTarget == null || dir == Vector3.zero) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(0, 0, angle);
        chargeTarget.transform.rotation = Quaternion.RotateTowards(chargeTarget.transform.rotation, targetRot, rotationFollowSpeed * Time.deltaTime);
    }

    private IEnumerator DamageReductionLoop()
    {
        while (true)
        {
            if (damageHandler != null)
            {
                float newAttack = damageHandler.GetAttackValue() - damageReductionValue;
                if (newAttack < minAttackValue)
                {
                    damageHandler.AddAttackBonus(minAttackValue - damageHandler.GetAttackValue());
                }
                else
                {
                    damageHandler.AddAttackBonus(-damageReductionValue);
                }
            }
            yield return new WaitForSeconds(damageReductionInterval);
        }
    }

    private void SetTransparency(float alpha)
    {
        if (bodyRenderer != null)
        {
            Color c = bodyRenderer.color;
            c.a = alpha;
            bodyRenderer.color = c;
        }
    }

    private IEnumerator ReactivateColliderAfterDelay()
{
    yield return new WaitForSeconds(colliderReactivateDelay);
    if (myCollider != null)
        myCollider.enabled = true;  // 다시 활성화
}


private void OnTriggerEnter(Collider other)
{
    if (!dealDamageWhileCharging) return;
    if (!other.CompareTag("Enemy")) return;

    HealthSystem hs = other.GetComponent<HealthSystem>();
    if (hs == null) return;

    // 데미지 처리
    hs.ApplyFullDamage(damageHandler.GetAttackValue());

    // 충돌 시 콜라이더 비활성화 및 일정 시간 후 다시 활성화
    if (myCollider != null)
    {
        myCollider.enabled = false;
        StartCoroutine(ReactivateColliderAfterDelay());
    }
}

}
