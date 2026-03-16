using System.Collections.Generic;

[System.Serializable]
public class MatchResultData
{
    public string WinnerName;
    public PredictionSide WinnerSide;
    public bool IsPredictionSuccess;
    public int RewardCoin;
    public List<string> BattleLogs;
}