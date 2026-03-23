using UnityEngine;

public class PlayerHealthSaveManager : MonoBehaviour
{
    public static PlayerHealthSaveManager Instance;

    private int savedHealth;
    private bool savedIsInvincible;
    private float savedInvincibleTimer;
    private bool hasSavedData = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 시에도 살아있게!
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 플레이어 체력과 무적 상태 저장
    /// </summary>
    public void SavePlayerState(PlayerHealthSystem player)
    {
        savedHealth = player.currentHealth;
        savedIsInvincible = player.GetInvincibleState();
        savedInvincibleTimer = player.GetInvincibleTimer();
        hasSavedData = true;

        Debug.Log($"📝 [PlayerHealthSaveManager] 상태 저장: 체력 {savedHealth}, 무적 {savedIsInvincible}, 무적 타이머 {savedInvincibleTimer}");
    }

    /// <summary>
    /// 저장된 상태를 플레이어에 복원
    /// </summary>
    public void RestorePlayerState(PlayerHealthSystem player)
    {
        if (!hasSavedData)
        {
            Debug.LogWarning("[PlayerHealthSaveManager] 저장된 상태가 없습니다. 복원 생략.");
            return;
        }

        player.currentHealth = savedHealth;
        player.SetInvincibleState(savedIsInvincible);
        player.SetInvincibleTimer(savedInvincibleTimer);
        player.UpdateHeartUI();

        Debug.Log($"✅ [PlayerHealthSaveManager] 상태 복원 완료: 체력 {savedHealth}, 무적 {savedIsInvincible}, 무적 타이머 {savedInvincibleTimer}");
    }

    /// <summary>
    /// 저장된 정보 초기화
    /// </summary>
    public void ClearSavedState()
    {
        hasSavedData = false;
    }
}
