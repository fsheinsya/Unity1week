using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Resultシーン全体を管理するクラス
/// 主な役割:
/// ・勝者名の表示
/// ・勝者画像の表示
/// ・現在コイン数の表示
/// ・現在ラウンド数の表示
/// ・画面クリックで次の試合 or 最終結果画面へ遷移
/// </summary>
public class ResultSceneController : MonoBehaviour
{
    [Header("結果表示UI")]
    [SerializeField] private TMP_Text winnerNameText;
    [SerializeField] private Image winnerImage;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text roundText;

    // 連打で複数回遷移しないようにするフラグ
    private bool isTransitioning = false;

    /// <summary>
    /// シーン開始時に呼ばれる
    /// GameSession に保存された試合結果を画面に反映する
    /// </summary>
    private void Start()
    {
        // GameSession が無いならタイトルへ戻す
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // 試合結果が無いならタイトルへ戻す
        if (GameSession.Instance.CurrentMatchResult == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // 画面表示を更新する
        SetupResultView();
    }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// クリック入力を検知して次へ進む
    /// </summary>
    private void Update()
    {
        // すでに遷移中なら何もしない
        if (isTransitioning) return;

        // 左クリックまたは右クリックで次へ進む
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            GoNext();
        }
    }

    /// <summary>
    /// 勝者名、勝者画像、コイン、ラウンド数を表示する
    /// </summary>
    private void SetupResultView()
    {
        // 今回の試合結果を取得
        MatchResultData result = GameSession.Instance.CurrentMatchResult;

        // 現在ラウンド表示
        roundText.text = $"round {GameSession.Instance.CurrentRound} / {GameSession.Instance.MaxRound}";

        // 現在コイン表示
        coinText.text = $"Coin\n{GameSession.Instance.CurrentCoin}";

        // 勝者モンスターを取得
        MonsterData winnerMonster = GetWinnerMonster(result.WinnerSide);

        // 勝者名表示
        winnerNameText.text = winnerMonster.Name;

        // 勝者画像表示
        if (winnerImage != null)
        {
            winnerImage.sprite = winnerMonster.Icon;
        }
    }

    /// <summary>
    /// 勝者が左右どちらかに応じて該当モンスターを返す
    /// </summary>
    private MonsterData GetWinnerMonster(PredictionSide winnerSide)
    {
        if (winnerSide == PredictionSide.Left)
        {
            return GameSession.Instance.LeftMonster;
        }

        return GameSession.Instance.RightMonster;
    }

    /// <summary>
    /// 次の画面へ進む
    /// 最終ラウンドなら AllResult、それ以外なら次の Bet に進む
    /// </summary>
    private void GoNext()
    {
        // 多重遷移防止
        isTransitioning = true;

        // 3試合目なら最終結果画面へ
        if (GameSession.Instance.IsLastRound())
        {
            SceneManager.LoadScene(SceneNames.AllResult);
            return;
        }

        // 次のラウンドへ進める
        GameSession.Instance.NextRound();

        // 次の賭け画面へ移動
        SceneManager.LoadScene(SceneNames.Bet);
    }
}