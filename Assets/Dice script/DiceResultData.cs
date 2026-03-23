public static class DiceResultData
{
    public static int diceValue = 0;
    public static int[] resultCounts = new int[6];
    public static bool isNewResult = false;

    public static void AddResultCount(int value)
    {
        if (value >= 1 && value <= resultCounts.Length)
        {
            resultCounts[value - 1]++;
        }
    }

    public static void ResetCounts()
    {
        diceValue = 0;
        isNewResult = false;
        for (int i = 0; i < resultCounts.Length; i++)
        {
            resultCounts[i] = 0;
        }
    }
}
