// 버튼 클릭시 씬이동을 하게 해주는 스크립트
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("씬 이름 설정")]
    public string mainMenuSceneName = "MainMenuScene";
    public string gameSceneName = "GameScene";

    // 시작 버튼에서 호출되는 함수
    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
