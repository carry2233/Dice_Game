using UnityEngine;

public class GunShooter : MonoBehaviour
{
    [Header("총알 프리팹")]
    public GameObject bulletPrefab;

    [Header("총알 생성 위치")]
    public Transform firePoint;

    [Header("발사 딜레이")]
    public float fireDelay = 0.2f;

    private float fireTimer = 0f;
    //private bool canShoot = true;

    private void Update()
    {
        // 마우스 우클릭 상태일 때만 발사 가능
        if (Input.GetMouseButton(1))
        {
            // 마우스 좌클릭 눌렀을 때
            if (Input.GetMouseButtonDown(0))
            {
                TryFire();
            }

            // 마우스 좌클릭 누르고 있는 동안 딜레이 체크
            if (Input.GetMouseButton(0))
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= fireDelay)
                {
                    TryFire();
                    fireTimer = 0f;
                }
            }
        }

        // 마우스 좌클릭에서 손 뗐을 때 타이머 초기화
        if (Input.GetMouseButtonUp(0))
        {
            fireTimer = 0f;
        }
    }

private void TryFire()
{
    if (bulletPrefab != null && firePoint != null)
    {
        // 오브젝트 풀링 사용
        GameObject bullet = SpawnOnDamagePoolManager.Instance.SpawnFromPool(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet != null)
        {
            //Debug.Log("🔫 총알 발사 (풀링 사용)!");
        }
        else
        {
            Debug.LogWarning("⚠️ 총알 풀에서 오브젝트를 가져올 수 없습니다!");
        }
    }
}

}