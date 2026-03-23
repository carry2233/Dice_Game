using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerHealthSystem : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 5;
    public int currentHealth = 5;

    [Header("하트 UI 리스트")]
    public List<HeartUIUnit> heartUIs = new List<HeartUIUnit>();
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    [Header("무적 설정")]
    public float invincibleDuration = 2f;
    public float blinkSpeed = 5f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;
    public List<Material> blinkMaterials = new List<Material>();

    private bool isInvincible = false;
    private float invincibleTimer = 0f;
    [Header("게임 오버 설정")]
public string gameOverSceneName = "GameOverScene";  // 이동할 씬 이름


    private void Start()
    {
        UpdateHeartUI();
        SetAlphaToMaterials(1f);  // 🟢 게임 시작 시 알파값 1로 초기화
        StartInvincibility();     // ✅ 시작하자마자 무적 실행
    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;

            // 깜빡임 처리
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * blinkSpeed, 1f));
            SetAlphaToMaterials(alpha);

            if (invincibleTimer >= invincibleDuration)
            {
                EndInvincibility();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHeartUI();

        if (currentHealth > 0)
        {
            StartInvincibility();
        }
        else
        {
            Debug.Log("플레이어 사망");
            // 사망 처리 추가 가능
        }
         CheckHealth();  // ✅ 이거 추가해야 동작함!
    }

    public void UpdateHeartUI()
    {
        foreach (HeartUIUnit heart in heartUIs)
        {
            if (heart.heartIndex <= currentHealth)
            {
                heart.SetHeartImage(fullHeartSprite);
            }
            else
            {
                heart.SetHeartImage(emptyHeartSprite);
            }
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibleTimer = 0f;
    }

    private void EndInvincibility()
    {
        isInvincible = false;
        invincibleTimer = 0f;
        SetAlphaToMaterials(1f); // 원상복구 (불투명)
    }

    private void SetAlphaToMaterials(float alpha)
    {
        foreach (var mat in blinkMaterials)
        {
            if (mat != null)
            {
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
    }

    // ✅ 체력 회복 메서드 (기존)
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHeartUI();
    }

    // ✅ 새로 추가된: 오브젝트 활성화 시 회복용
    public void HealOnObjectActivation(int healAmount)
    {
        Heal(healAmount); // 최대 체력 초과 방지 포함
    }

    // ✅ 무적 상태 확인/설정 메서드들
    public bool GetInvincibleState()
    {
        return isInvincible;
    }

public void SetInvincibleState(bool value)
{
    if (value)
    {
        isInvincible = true;
        invincibleTimer = 0f;
    }
    else
    {
        EndInvincibility();   // ✅ 무적 해제 시 EndInvincibility 호출!
    }
}


    public float GetInvincibleTimer()
    {
        return invincibleTimer;
    }

    public void SetInvincibleTimer(float value)
    {
        invincibleTimer = value;
    }

    // 체력 감소 또는 데미지 적용 부분에서 호출될 함수
public void CheckHealth()
{
    if (currentHealth <= 0)
    {
        OnPlayerDeath();
    }
}

// 체력 0 되었을 때 처리
private void OnPlayerDeath()
{
    // ✅ 주사위 기록 초기화
    DiceResultTracker.Instance.ResetCounts();

    if (WaveSpawner.Instance != null)
    {
        WaveSpawner.Instance.ClearSpawnerData();
    }

    SceneStateManager.Instance.ClearAllSavedStates();
    SceneManager.LoadScene(gameOverSceneName);
    Destroy(gameObject);
}


public void ApplyDamage(int damage)
{
    if (isInvincible)
        return;

    currentHealth -= damage;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    UpdateHeartUI();
    CheckHealth();  // ✅ 체력 체크 추가
}



}
