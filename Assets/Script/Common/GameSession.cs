using UnityEngine;

/// <summary>
/// シーンをまたいでゲーム全体の状態を保持するクラス
/// 主な役割:
/// ・所持コインの管理
/// ・現在ラウンド数の管理
/// ・左右モンスター情報の保持
/// ・賭け情報の保持
/// ・試合結果の保持
/// </summary>
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    // 現在の所持コイン
    public int CurrentCoin { get; private set; } = 100;

    // 現在何試合目か
    public int CurrentRound { get; private set; } = 1;

    // 最大試合数
    public int MaxRound { get; private set; } = 3;

    // 現在の左モンスター
    public MonsterData LeftMonster { get; private set; }

    // 現在の右モンスター
    public MonsterData RightMonster { get; private set; }

    // 現在の賭け情報
    public BetData CurrentBet { get; private set; }

    // 直前の試合結果
    public MatchResultData CurrentMatchResult { get; private set; }

    // モンスター生成用クラス
    private MonsterFactory monsterFactory;

    /// <summary>
    /// 最初に1回だけ生成される
    /// 重複生成を防ぐ
    /// </summary>
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

    /// <summary>
    /// 新しいゲームを開始する
    /// タイトルから開始するときに呼ぶ
    /// </summary>
    public void StartNewGame()
    {
        // 初期コインに戻す
        CurrentCoin = 100;

        // 1試合目に戻す
        CurrentRound = 1;

        // 最大3試合固定
        MaxRound = 3;

        // 賭け情報と結果を初期化
        CurrentBet = null;
        CurrentMatchResult = null;

        // 最初の対戦カードを生成する
        GenerateNewMatch();
    }

    /// <summary>
    /// 新しい対戦カードを生成する
    /// </summary>
    public void GenerateNewMatch()
    {
        // 左右のモンスターを新しく作る
        LeftMonster = monsterFactory.CreateRandom("Left Monster");
        RightMonster = monsterFactory.CreateRandom("Right Monster");

        // 前回の賭け情報と結果は消しておく
        CurrentBet = null;
        CurrentMatchResult = null;
    }

    /// <summary>
    /// 現在の賭け情報を保存する
    /// </summary>
    public void SetBet(BetData betData)
    {
        CurrentBet = betData;
    }

    /// <summary>
    /// コインを消費する
    /// 足りない場合は false を返す
    /// </summary>
    public bool TryConsumeCoin(int amount)
    {
        // マイナス値は無効
        if (amount < 0) return false;

        // 所持金不足なら失敗
        if (CurrentCoin < amount) return false;

        // コインを減らす
        CurrentCoin -= amount;
        return true;
    }

    /// <summary>
    /// コインを加算する
    /// </summary>
    public void AddCoin(int amount)
    {
        // マイナス加算はしない
        if (amount < 0) return;

        CurrentCoin += amount;
    }

    /// <summary>
    /// 今回の試合結果を保存する
    /// </summary>
    public void SetMatchResult(MatchResultData result)
    {
        CurrentMatchResult = result;
    }

    /// <summary>
    /// 今が最終試合かどうかを返す
    /// </summary>
    public bool IsLastRound()
    {
        return CurrentRound >= MaxRound;
    }

    /// <summary>
    /// 次の試合へ進む
    /// ラウンド数を1つ進めて、新しい対戦カードを作る
    /// </summary>
    public void NextRound()
    {
        // 次の試合番号へ進める
        CurrentRound++;

        // 新しい対戦カードを生成する
        GenerateNewMatch();
    }
}