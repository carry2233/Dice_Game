using UnityEngine;
using System.Collections.Generic;

public class ChaseRangeLimiter : MonoBehaviour
{
    [Header("범위 설정")]
    public float detectionRadius = 5f;

    private Transform nearestTarget;

    [Header("AI 리스트")]
    public List<ChaseControllable> aiControllers = new();

    private void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        nearestTarget = FindNearest(players);

        if (nearestTarget == null)
        {
            Debug.LogWarning("[ChaseRangeLimiter] Player 태그를 가진 오브젝트를 찾을 수 없습니다.");
            return;
        }

        float targetDistance = Vector3.Distance(transform.position, nearestTarget.position);
        bool isTargetInRange = targetDistance <= detectionRadius;

        if (isTargetInRange)
        {
            Debug.Log($"[ChaseRangeLimiter] ✅ Player '{nearestTarget.name}' 감지 범위 안에 있음 (거리: {targetDistance:F2})");
        }

        foreach (var ai in aiControllers)
        {
            if (ai == null) continue;

            float aiDistance = Vector3.Distance(transform.position, ai.transform.position);
            bool isAIInRange = aiDistance <= detectionRadius;

            bool shouldChase = isTargetInRange && isAIInRange;

            ai.SetChaseEnabled(shouldChase);

            if (shouldChase)
            {
                ai.SetTarget(nearestTarget);
                Debug.Log($"[ChaseRangeLimiter] ✅ AI '{ai.name}' 와 Player '{nearestTarget.name}' 모두 범위 안에 있음 → 추적 허용");
            }
            else
            {
                ai.SetTarget(null); // 타겟 제거
                if (!isAIInRange)
                    Debug.Log($"[ChaseRangeLimiter] ⛔ AI '{ai.name}' 이 범위 밖에 있음 → 추적 차단");
                if (!isTargetInRange)
                    Debug.Log($"[ChaseRangeLimiter] ⛔ Player '{nearestTarget.name}' 이 범위 밖에 있음 → 추적 차단");
            }
        }
    }

    private Transform FindNearest(GameObject[] players)
    {
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p.transform;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
