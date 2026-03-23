using UnityEngine;
using TMPro;

public class ResultReceiver : MonoBehaviour
{
    [Header("주사위 결과 텍스트")]
    public TMP_Text resultText;

    [Header("결과 숫자별 오브젝트")]
    public GameObject[] resultObjects; // 1~6번 오브젝트 (0번이 1번)

    [Header("두 번째 나왔을 때 활성화할 오브젝트")]
    public GameObject[] secondTimeActivationObjects;

    private void Start()
    {
        ActivateObjects();  // 씬 로드 시 자동으로 오브젝트 활성화
    }

    // 🟢 오브젝트 활성화 로직
    private void ActivateObjects()
    {
        int value = DiceResultData.diceValue;

        if (resultText != null)
            resultText.text = "주사위 결과: " + value;

        // ✅ 모든 오브젝트 비활성화
        foreach (GameObject obj in resultObjects)
            if (obj != null) obj.SetActive(false);

        foreach (GameObject obj in secondTimeActivationObjects)
            if (obj != null) obj.SetActive(false);

        // ✅ 값이 정상 범위라면 활성화 시도
        if (value >= 1 && value <= resultObjects.Length)
        {
            int count = DiceResultTracker.Instance.GetCount(value);

            if (count >= 2) // 두 번째 이상 나왔을 때
            {
                if (value <= secondTimeActivationObjects.Length)
                {
                    var secondObj = secondTimeActivationObjects[value - 1];
                    if (secondObj != null)
                        secondObj.SetActive(true); // 두 번째 등장 오브젝트만 활성화
                }
            }
            else // 첫 번째 등장일 때
            {
                var firstObj = resultObjects[value - 1];
                if (firstObj != null)
                    firstObj.SetActive(true); // 첫 번째 오브젝트만 활성화
            }

            Debug.Log($"[ResultReceiver] diceValue = {value}, count = {count}");
        }
        else
        {
            Debug.LogWarning($"[ResultReceiver] diceValue {value}가 resultObjects/secondTimeActivationObjects 범위를 벗어났습니다.");
        }
    }
}
