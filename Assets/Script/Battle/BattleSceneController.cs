using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Battleシーン全体の進行を管理するクラス
/// 主な役割:
/// ・現在の賭け情報を取得
/// ・バトルシミュレーションを実行
/// ・ログを順番に表示
/// ・結果を GameSession に保存
/// ・Result シーンへ移動
/// </summary>
public class BattleSceneController : MonoBehaviour
{
    [Header("ログ表示")]
    [SerializeField] private ActionLogView actionLogView;

    // 戦闘シミュレーションを担当するクラス
    private BattleSimulator battleSimulator;

    /// <summary>
    /// シーン開始時に非同期で戦闘を進行する
    /// </summary>
    private async void Start()
    {
        // シミュレーターを作成
        battleSimulator = new BattleSimulator();

        // セッション情報を取得
        GameSession session = GameSession.Instance;

        // セッションや賭け情報が存在しない場合は安全のため Bet に戻す
        if (session == null || session.CurrentBet == null)
        {
            SceneManager.LoadScene(SceneNames.Bet);
            return;
        }

        // 賭け金をここで消費する
        // 賭けに失敗した場合はそのまま没収される設計
        bool success = session.TryConsumeCoin(session.CurrentBet.Amount);
        if (!success)
        {
            // コイン不足なら賭けが成立しないので Bet に戻す
            SceneManager.LoadScene(SceneNames.Bet);
            return;
        }

        // ログ欄を初期化
        actionLogView.Clear();

        // 少し待ってから戦闘開始演出っぽくする
        await UniTask.Delay(500);

        // 左右モンスターと賭け情報を使って戦闘結果を計算する
        MatchResultData result = battleSimulator.Simulate(
            session.LeftMonster,
            session.RightMonster,
            session.CurrentBet
        );

        // 戦闘ログを1行ずつ順番に表示する
        foreach (string log in result.BattleLogs)
        {
            await actionLogView.AppendLineAnimated(log);
            await UniTask.Delay(350);
        }

        // 予想成功なら報酬コインを追加する
        if (result.IsPredictionSuccess)
        {
            session.AddCoin(result.RewardCoin);
            await actionLogView.AppendLineAnimated($"予想的中！ {result.RewardCoin} コイン獲得！");
        }
        else
        {
            await actionLogView.AppendLineAnimated("予想失敗……掛け金は没収された。");
        }

        // 今回の試合結果をセッションに保存する
        session.SetMatchResult(result);

        // 少し待ってから結果画面へ移動する
        await UniTask.Delay(1000);
        SceneManager.LoadScene(SceneNames.Result);
    }
}