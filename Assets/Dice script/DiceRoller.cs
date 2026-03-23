using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DiceRoller : MonoBehaviour
{
    public Rigidbody diceRb;

    [Header("던지기 힘 설정")]
    public float minPower = 5f;
    public float maxPower = 10f;

    [Header("회전 힘 설정")]
    public float minTorque = 200f;
    public float maxTorque = 400f;

    [Header("주사위 시작 위치/회전")]
    public Vector3 startPosition = new Vector3(0, 1f, 0);
    public Vector3 startRotation = Vector3.zero;

    [Header("면 정보")]
    public Transform[] facePoints;
    public TMP_Text diceResultText;

    [Header("굴리기 버튼")]
    public Button rollButton;
    public CanvasGroup rollButtonCanvasGroup;

    [Header("돌아가기 버튼")]
    public Button backButton;
    public CanvasGroup backButtonCanvasGroup;

    public string targetSceneName = "ResultScene";

    private bool hasStopped = false;
    private bool isRolling = false;
    private int diceResult = 0;

    public float stopVelocityThreshold = 0.05f;
    public float stopAngularVelocityThreshold = 0.05f;

    void Start()
    {
        // 초기 상태 설정
        rollButtonCanvasGroup.alpha = 1f;
        rollButton.interactable = true;

        backButtonCanvasGroup.alpha = 0f;
        backButton.interactable = false;

        diceResultText.gameObject.SetActive(false);
    }

    public void RollDice()
    {
        if (isRolling) return;

        // 텍스트 숨기기
        diceResultText.gameObject.SetActive(false);

        // 돌아가기 버튼 숨기기
        backButton.interactable = false;
        backButtonCanvasGroup.alpha = 0f;

        // 굴리기 버튼 비활성화 및 숨기기
        rollButton.interactable = false;
        rollButtonCanvasGroup.alpha = 0f;

        // 물리 초기화 및 위치 리셋
        diceRb.velocity = Vector3.zero;
        diceRb.angularVelocity = Vector3.zero;
        diceRb.transform.position = startPosition;
        diceRb.transform.rotation = Quaternion.Euler(startRotation);

        hasStopped = false;

        float power = Random.Range(minPower, maxPower);

        // ✅ 수정된 부분: 회전 방향 랜덤, 회전 세기 랜덤
        Vector3 randomDirection = Random.onUnitSphere; // 회전 방향 랜덤
        float randomTorqueMagnitude = Random.Range(minTorque, maxTorque); // 회전 세기 랜덤
        Vector3 torque = randomDirection * randomTorqueMagnitude;

        diceRb.AddForce(Vector3.up * power, ForceMode.Impulse);
        diceRb.AddTorque(torque, ForceMode.Impulse);

        Debug.Log($"🎲 파워: {power}, 회전 방향: {randomDirection}, 회전 세기: {randomTorqueMagnitude}");

        StartCoroutine(EnableRollingAfterDelay(1f));
    }

    IEnumerator EnableRollingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isRolling = true;
    }

    void Update()
    {
        if (isRolling &&
            !hasStopped &&
            diceRb.velocity.magnitude < stopVelocityThreshold &&
            diceRb.angularVelocity.magnitude < stopAngularVelocityThreshold)
        {
            hasStopped = true;
            isRolling = false;

            diceResult = GetTopFaceIndex();
            diceResultText.text = "" + diceResult;

            // 텍스트 보이기
            diceResultText.gameObject.SetActive(true);

            Debug.Log("🎯 숫자 판정 결과: " + diceResult);

            StartCoroutine(ShowBackButtonWithDelay(2f));
        }
    }

    IEnumerator ShowBackButtonWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 돌아가기 버튼 활성화
        backButtonCanvasGroup.alpha = 1f;
        backButton.interactable = true;
    }

    public void ReturnToResultScene()
    {
        DiceResultData.diceValue = diceResult;

        LogAllDiceCounts();

        // ✅ 결과값 기록
        DiceResultTracker.Instance.RecordDiceResult(diceResult);

        SceneManager.LoadScene(targetSceneName);
    }

    int GetTopFaceIndex()
    {
        float maxY = float.MinValue;
        int topIndex = -1;

        for (int i = 0; i < facePoints.Length; i++)
        {
            float yPos = facePoints[i].position.y;

            if (yPos > maxY)
            {
                maxY = yPos;
                topIndex = i;
            }
        }

        Debug.Log("🔍 가장 위에 있는 면 (position.y 기준): " + (topIndex + 1));
        return topIndex + 1;
    }

    // ✅ 1~6 주사위 등장 횟수 및 중복 여부 전체 출력
private void LogAllDiceCounts()
{
    Debug.Log("🎯 [중복 현황 출력 시작]");

    for (int i = 1; i <= 6; i++)
    {
        int count = DiceResultTracker.Instance.GetCount(i);
        string status = count >= 2 ? "✅ 중복!" : "🆕 첫 등장 또는 1회";
        Debug.Log($"🎲 {i} → {count}회 등장 ({status})");
    }

    Debug.Log("🟰 [중복 현황 출력 끝]");
}

}
