using UnityEngine;
using System.Collections.Generic;

public class DiceResultTracker : MonoBehaviour
{
    public static DiceResultTracker Instance;
    private Dictionary<int, int> diceCounts = new Dictionary<int, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecordDiceResult(int value)
    {
        if (!diceCounts.ContainsKey(value))
            diceCounts[value] = 0;

        diceCounts[value]++;
        Debug.Log($"🎲 [DiceResultTracker] 주사위 {value}번 {diceCounts[value]}회 등장");
    }

    public int GetCount(int value)
    {
        return diceCounts.ContainsKey(value) ? diceCounts[value] : 0;
    }

    public void ResetCounts()
    {
        diceCounts.Clear();
        Debug.Log("♻️ [DiceResultTracker] 주사위 카운트 초기화 완료");
    }
}
