using UnityEngine;
using System.Collections.Generic;

public class ZoneEntryBlocker : MonoBehaviour
{
    [System.Serializable]
    public class BlockZone
    {
        public string name = "BlockZone";
        public Vector3 minBounds = new Vector3(-5, -5, -5);
        public Vector3 maxBounds = new Vector3(5, 5, 5);
    }

    [Header("차단 구역 리스트")]
    public List<BlockZone> blockZones = new List<BlockZone>();

    private Vector3 lastSafePosition;

    private void Start()
    {
        lastSafePosition = transform.position;
    }

    private void LateUpdate()
    {
        bool isInsideAnyZone = false;

        foreach (var zone in blockZones)
        {
            if (IsInsideZone(transform.position, zone))
            {
                isInsideAnyZone = true;
                break;
            }
        }

        if (isInsideAnyZone)
        {
            // ✅ 진입 시도 → 이전 위치로 되돌리기
            transform.position = lastSafePosition;
        }
        else
        {
            // ✅ 현재 위치가 안전 → 위치 저장
            lastSafePosition = transform.position;
        }
    }

    private bool IsInsideZone(Vector3 pos, BlockZone zone)
    {
        return pos.x >= zone.minBounds.x && pos.x <= zone.maxBounds.x &&
               pos.y >= zone.minBounds.y && pos.y <= zone.maxBounds.y &&
               pos.z >= zone.minBounds.z && pos.z <= zone.maxBounds.z;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);

        foreach (var zone in blockZones)
        {
            Vector3 center = (zone.minBounds + zone.maxBounds) * 0.5f;
            Vector3 size = zone.maxBounds - zone.minBounds;

            Gizmos.DrawCube(center, size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, size);
        }
    }
}
