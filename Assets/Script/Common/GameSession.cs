using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    public int CurrentCoin { get; private set; } = 100;
    public int CurrentRound { get; private set; } = 1;
    public int MaxRound { get; private set; } = 3;

    public MonsterData LeftMonster { get; private set; }
    public MonsterData RightMonster { get; private set; }

    public BetData CurrentBet { get; private set; }
    public MatchResultData CurrentMatchResult { get; private set; }

    private MonsterFactory monsterFactory;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        monsterFactory = new MonsterFactory();
    }

    public void StartNewGame()
    {
        //初期化
        CurrentCoin = 100;
        CurrentRound = 1;
        MaxRound = 3;

        CurrentBet = null;
        CurrentMatchResult = null;

        GenerateNewMatch();
    }

    public void GenerateNewMatch()
    {
        LeftMonster = monsterFactory.CreateRandom("Left Monster");
        RightMonster = monsterFactory.CreateRandom("Right Monster");

        CurrentBet = null;
        CurrentMatchResult = null;
    }

    public void SetBet(BetData betData)
    {
        CurrentBet = betData;
    }

    public bool TryConsumeCoin(int amount)
    {
        if (amount < 0) return false;
        if (CurrentCoin < amount) return false;

        CurrentCoin -= amount;
        return true;
    }

    public void AddCoin(int amount)
    {
        if (amount < 0) return;
        CurrentCoin += amount;
    }

    public void SetMatchResult(MatchResultData result)
    {
        CurrentMatchResult = result;
    }

    public bool IsLastRound()
    {
        return CurrentRound >= MaxRound;
    }

    public void NextRound()
    {
        CurrentRound++;
        GenerateNewMatch();
    }
}
