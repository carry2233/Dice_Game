using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StickAttack : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject attackPrefab; // 생성할 프리팹
    public Transform targetObject;  // 기준이 되는 대상 오브젝트 (공전 중심)

    [Header("Attack Properties")]
    public float startAngleOffset = 0f;  // 타겟 오브젝트의 방향에 더할 공전 시작 각도
    public float distanceFromTarget = 1f; // 기준 오브젝트와의 거리
    public float rotationSpeed = 100f;    // 공전 속도 (도/초)
    public float activeTime = 2f;         // 활성화 시간
    public float lookAtAngleOffset = 0f;  // 프리팹의 방향에 더할 각도

    [Header("Pooling Settings")]
    public int poolSize = 10;          // 오브젝트 풀 크기

    private Queue<GameObject> attackPool; // 오브젝트 풀
    private bool isRightClickHeld = false; // 우클릭 상태 확인
    private bool isAttackActive = false;  // 현재 공격이 활성화된 상태인지 확인

    private Renderer objectRenderer; // 스크립트가 적용된 오브젝트의 Renderer

    [Header("Attack Delay Settings")]
public float attackDelay = 1.0f;  // 공격 간 딜레이 시간
private float attackTimer = 0f;   // 딜레이 측정용 타이머

[Header("공격력 보너스 설정")]
public bool enableRotationBasedBonus = true; // 회전 기반 공격력 증가 허용 여부

[Header("속도 비례 추가 공격력 설정")]
public float rotationBonusMultiplier = 0.1f; // 회전속도 기반 보너스 배율




    private void Start()
    {
        InitializeAttackPool();

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogWarning("Renderer가 이 오브젝트에 존재하지 않습니다!");
        }
    }

private void Update()
{
    isRightClickHeld = Input.GetMouseButton(1);
    attackTimer += Time.deltaTime;

    if (isRightClickHeld && Input.GetMouseButton(0) && !isAttackActive && attackTimer >= attackDelay)
    {
        FireAttack();
        attackTimer = 0f; // ✅ 공격 직후 타이머 리셋
    }
}


    private void FireAttack()
    {
        if (attackPool == null || targetObject == null) return;

        GameObject attackInstance = GetPooledAttack();
        if (attackInstance != null)
        {
            isAttackActive = true;

            float targetAngle = GetTargetRotationAngle() + startAngleOffset;
            Vector3 initialPosition = targetObject.position +
                (Quaternion.Euler(0, 0, targetAngle) * Vector3.right * distanceFromTarget);
            attackInstance.transform.position = initialPosition;

            attackInstance.SetActive(true);

            if (objectRenderer != null)
            {
                objectRenderer.enabled = false;
            }

            StartCoroutine(OrbitAndDeactivate(attackInstance, targetAngle));
        }
        else
        {
            Debug.LogWarning("오브젝트 풀에 남은 공격 프리팹이 없습니다!");
        }
    }

private IEnumerator OrbitAndDeactivate(GameObject attackInstance, float startAngle)
{
    float elapsedTime = 0f;
    float currentAngle = startAngle;
    float lastTargetAngle = GetTargetRotationAngle();

    while (elapsedTime < activeTime)
    {
        float currentTargetAngle = GetTargetRotationAngle();
        float angleDifference = Mathf.DeltaAngle(lastTargetAngle, currentTargetAngle);

        currentAngle += angleDifference;
        currentAngle -= rotationSpeed * Time.deltaTime;

        Vector3 offset = Quaternion.Euler(0, 0, currentAngle) * Vector3.right * distanceFromTarget;
        attackInstance.transform.position = targetObject.position + offset;

        Vector3 directionToTarget = targetObject.position - attackInstance.transform.position;
        float lookAtAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        attackInstance.transform.rotation = Quaternion.Euler(0, 0, lookAtAngle + lookAtAngleOffset);

        // ✅ 회전량에 따라 공격력 보정
DamageHandler handler = attackInstance.GetComponent<DamageHandler>();
if (handler != null && enableRotationBasedBonus)
{
    float bonus = Mathf.Abs(angleDifference) * rotationBonusMultiplier;
    handler.AddAttackBonus(bonus);
}


        lastTargetAngle = currentTargetAngle;
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // ✅ 공격력 초기화
    DamageHandler finalHandler = attackInstance.GetComponent<DamageHandler>();
    if (finalHandler != null)
    {
        finalHandler.ResetAttackValue();
    }

    attackInstance.SetActive(false);
    attackPool.Enqueue(attackInstance);

    if (objectRenderer != null)
    {
        objectRenderer.enabled = true;
    }

    isAttackActive = false;
}


    private void InitializeAttackPool()
    {
        attackPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject attackInstance = Instantiate(attackPrefab);
            attackInstance.SetActive(false);
            attackPool.Enqueue(attackInstance);
        }
    }

    private GameObject GetPooledAttack()
    {
        if (attackPool.Count > 0)
        {
            return attackPool.Dequeue();
        }
        return null;
    }

    private float GetTargetRotationAngle()
    {
        Vector3 forward = targetObject.right;
        return Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
    }
}
