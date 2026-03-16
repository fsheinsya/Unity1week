using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 1試合ごとの結果画面を管理するクラス
/// 主な役割:
/// ・今回の試合結果を表示
/// ・所持コインを表示
/// ・次の試合へ進む、または最終結果へ進む
/// </summary>
public class ResultSceneController : MonoBehaviour
{
    [Header("結果表示UI")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text roundText;

    /// <summary>
    /// シーン開始時に今回の試合結果をUIへ表示する
    /// </summary>
    private void Start()
    {
        GameSession session = GameSession.Instance;

        // セッションまたは試合結果がない場合は安全のため Title に戻す
        if (session == null || session.CurrentMatchResult == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        MatchResultData result = session.CurrentMatchResult;

        // 現在のラウンド情報を表示
        roundText.text = $"Round : {session.CurrentRound} / {session.MaxRound}";

        // 現在の所持コインを表示
        coinText.text = $"Coin : {session.CurrentCoin}";

        // 予想が当たったかどうかで表示文を変える
        if (result.IsPredictionSuccess)
        {
            resultText.text = $"{result.WinnerName} の勝利！\n予想成功！";
        }
        else
        {
            resultText.text = $"{result.WinnerName} の勝利！\n予想失敗……";
        }
    }

    /// <summary>
    /// Nextボタンから呼ばれる
    /// 最終試合なら AllResult へ、それ以外なら次の Bet シーンへ進む
    /// </summary>
    public void OnClickNext()
    {
        GameSession session = GameSession.Instance;

        // 最終試合なら最終結果シーンへ移動
        if (session.IsLastRound())
        {
            SceneManager.LoadScene(SceneNames.AllResult);
            return;
        }

        // 次のラウンドへ進めて、新しい対戦カードを作る
        session.NextRound();

        // 再び賭け画面へ戻る
        SceneManager.LoadScene(SceneNames.Bet);
    }
}