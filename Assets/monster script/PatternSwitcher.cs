using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatternSwitcher : MonoBehaviour
{
    [Header("패턴 리스트 (Monster, TimedChargeAI 등)")]
    public List<MonoBehaviour> patternList = new List<MonoBehaviour>();

    [Header("패턴 유지 시간")] 
    public float patternDuration = 3f;

    [Header("패턴 전환 딜레이 (모두 비활성화 상태)")]
    public float patternDelay = 1f;

    private int currentPatternIndex = -1; // -1: 아무 패턴도 없는 상태

    private void Start()
    {
        StartCoroutine(PatternCycle());
    }

    private IEnumerator PatternCycle()
    {
        while (true)
        {
            DisableAllPatterns(); // ✅ 전환 시작 시 모두 비활성화

            yield return new WaitForSeconds(patternDelay); // ✅ 딜레이 중 전부 OFF 상태

            int nextIndex = Random.Range(0, patternList.Count); // ✅ 랜덤 선택
            patternList[nextIndex].enabled = true;
            currentPatternIndex = nextIndex;

            yield return new WaitForSeconds(patternDuration); // ✅ 유지 시간 동안 활성화
        }
    }

    private void DisableAllPatterns()
    {
        foreach (var pattern in patternList)
        {
            if (pattern != null)
                pattern.enabled = false;
        }
    }

    // ✅ (옵션) 현재 어떤 패턴이 활성화 중인지 외부에서 확인용
    public int GetCurrentPatternIndex()
    {
        return currentPatternIndex;
    }
}
