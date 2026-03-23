using UnityEngine;
using System.Collections;

public class ShotgunShooter : MonoBehaviour
{
    [Header("총알 프리팹 (풀링 대상)")]
    public GameObject bulletPrefab;

    [Header("총알 생성 위치")]
    public Transform firePoint;

    [Header("발사 딜레이")]
    public float fireDelay = 0.5f;
    private bool canShoot = true;

    [Header("샷건 설정")]
    public int bulletCount = 5;
    public float spreadAngle = 30f;

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                TryFire();
            }
        }
    }

    private void TryFire()
    {
        if (!canShoot || bulletPrefab == null || firePoint == null) return;

        FireShotgun();
        StartCoroutine(FireDelayCoroutine());
    }

    private void FireShotgun()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angleOffset = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            Quaternion spreadRot = firePoint.rotation * Quaternion.Euler(0f, 0f, angleOffset);

            GameObject bullet = SpawnOnDamagePoolManager.Instance.SpawnFromPool(
                bulletPrefab,
                firePoint.position,
                spreadRot
            );

            if (bullet != null)
            {
                bullet.transform.right = spreadRot * Vector3.right;
            }
        }
    }

    private IEnumerator FireDelayCoroutine()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireDelay);
        canShoot = true;
    }
}
