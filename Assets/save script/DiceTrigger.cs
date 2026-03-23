using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DiceTrigger : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    public string targetSceneName = "BScene";

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(SaveAndLoadScene());
    }

    private IEnumerator SaveAndLoadScene()
    {
        Debug.Log("🎲 [DiceTrigger] 상태 저장 대기 시작");

        yield return new WaitForEndOfFrame(); // 프레임 종료 시점까지 대기

        // ✅ 몬스터 및 기타 오브젝트 상태 저장
        SceneStateManager.Instance.StoreAllStates();

        // ✅ 플레이어 체력/무적 상태 저장
        var player = FindObjectOfType<PlayerHealthSystem>();
        if (player != null)
        {
            PlayerHealthSaveManager.Instance.SavePlayerState(player);
        }
        else
        {
            Debug.LogWarning("⚠️ [DiceTrigger] 플레이어를 찾을 수 없습니다.");
        }

        // ✅ 🌟 웨이브 스포너 상태 저장 (이 부분이 새로 추가된 핵심!)
        SceneStateManager.Instance.StoreWaveSpawnerState();

        Debug.Log("🎲 [DiceTrigger] 상태 저장 완료");

        // ✅ 씬 복귀 플래그 저장
        PlayerPrefs.SetInt("ReturnedFromBScene", 1);
        PlayerPrefs.Save();

        // ✅ 씬 이동
        SceneManager.LoadScene(targetSceneName);
    }
}
