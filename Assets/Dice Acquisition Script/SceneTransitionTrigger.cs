//이거 안씀
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string nextSceneName = "GameOverScene";
    public LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("▶️ 플레이어가 감지 영역에 진입! 씬 전환");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
