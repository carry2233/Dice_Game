using UnityEngine;

public abstract class ChaseControllable : MonoBehaviour
{
    protected Transform target;
    protected bool isChaseEnabled = true;
    protected float maxChaseDistance = 10f;

    public virtual void SetChaseEnabled(bool enabled)
    {
        isChaseEnabled = enabled;
    }

    public virtual void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    /// <summary>
    /// 특정 범위 내 가장 가까운 플레이어 탐색
    /// </summary>
    protected void FindNearestPlayerInRadius(Vector3 center, float radius)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(center, p.transform.position);
            if (dist <= radius && dist < minDist)
            {
                minDist = dist;
                nearest = p.transform;
            }
        }

        target = nearest;
    }

    /// <summary>
    /// 거리 제한 없이 가장 가까운 플레이어 탐색
    /// </summary>
    protected void FindNearestPlayerAnywhere()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float minDist = Mathf.Infinity;
        Transform nearest = null;

        foreach (GameObject p in players)
        {
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = p.transform;
            }
        }

        target = nearest;
    }
}
