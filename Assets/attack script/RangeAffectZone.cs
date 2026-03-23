using UnityEngine;
using System.Collections.Generic;

public class RangeAffectZone : MonoBehaviour
{
    [Header("영향 범위 설정")]
    public float radius = 5f;

    private List<Monster> monstersInZone = new();
    private List<TimedChargeAI> chargersInZone = new();

    private void Update()
    {
        // 몬스터 검색
        Monster[] allMonsters = GameObject.FindObjectsOfType<Monster>();
        foreach (Monster monster in allMonsters)
        {
            float dist = Vector3.Distance(transform.position, monster.transform.position);
            bool isInZone = dist <= radius;

            if (isInZone && !monstersInZone.Contains(monster))
            {
                monster.ApplySpeedModifier(0.5f);
                monstersInZone.Add(monster);
            }
            else if (!isInZone && monstersInZone.Contains(monster))
            {
                monster.ResetSpeed();
                monstersInZone.Remove(monster);
            }
        }

        // 돌진 AI 검색
        TimedChargeAI[] allChargers = GameObject.FindObjectsOfType<TimedChargeAI>();
        foreach (TimedChargeAI charger in allChargers)
        {
            float dist = Vector3.Distance(transform.position, charger.transform.position);
            bool isInZone = dist <= radius;

            if (isInZone && !chargersInZone.Contains(charger))
            {
                charger.isChargeBlocked = true;
                chargersInZone.Add(charger);
            }
            else if (!isInZone && chargersInZone.Contains(charger))
            {
                charger.isChargeBlocked = false;
                chargersInZone.Remove(charger);
            }
        }
    }

    private void OnDisable()
    {
        // 🔁 오브젝트가 꺼질 때 모든 상태 복구
        foreach (var monster in monstersInZone)
        {
            if (monster != null)
                monster.ResetSpeed();
        }
        monstersInZone.Clear();

        foreach (var charger in chargersInZone)
        {
            if (charger != null)
                charger.isChargeBlocked = false;
        }
        chargersInZone.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
