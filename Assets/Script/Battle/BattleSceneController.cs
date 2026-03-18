using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Battleシーン全体を管理するクラス
/// ・左右モンスターの表示
/// ・賭け金の消費
/// ・バトル実行
/// ・ログ表示
/// ・Resultシーンへの遷移
/// を担当する
/// </summary>
public class BattleSceneController : MonoBehaviour
{
    [Header("上部表示")]
    [SerializeField] private TMP_Text coinText;

    [Header("左キャラ表示")]
    [SerializeField] private Image leftCharaImage;
    [SerializeField] private BattleStatusView leftStatusView;

    [Header("右キャラ表示")]
    [SerializeField] private Image rightCharaImage;
    [SerializeField] private BattleStatusView rightStatusView;

    [Header("ログ表示")]
    [SerializeField] private ActionLogView actionLogView;

    // 戦闘計算本体
    private BattleSimulator battleSimulator;

    /// <summary>
    /// シーン開始時に呼ばれる
    /// Battleシーンに入ったら自動で戦闘を始める
    /// </summary>
    private async void Start()
    {
        // セッションが存在しない場合はTitleへ戻す
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // 賭け情報がない場合はBetへ戻す
        if (GameSession.Instance.CurrentBet == null)
        {
            SceneManager.LoadScene(SceneNames.Bet);
            return;
        }

        // BattleSimulator を作成する
        battleSimulator = new BattleSimulator();

        // 戦闘開始前の表示を整える
        SetupBattleView();

        // 少し待ってから開始すると見栄えが良い
        await UniTask.Delay(500);

        // 実際の戦闘処理を開始する
        await PlayBattleAsync();
    }

    /// <summary>
    /// Battle開始前のUI初期表示を行う
    /// </summary>
    private void SetupBattleView()
    {
        MonsterData leftMonster = GameSession.Instance.LeftMonster;
        MonsterData rightMonster = GameSession.Instance.RightMonster;

        // 上部のコイン表示を更新する
        coinText.text = $"nokorikakekin:{GameSession.Instance.CurrentCoin}";

        // 左右キャラ画像を表示する
        if (leftCharaImage != null)
        {
            leftCharaImage.sprite = leftMonster.Icon;
        }

        if (rightCharaImage != null)
        {
            rightCharaImage.sprite = rightMonster.Icon;
        }

        // 左右ステータス欄を初期表示する
        if (leftStatusView != null)
        {
            leftStatusView.Show(leftMonster);
        }

        if (rightStatusView != null)
        {
            rightStatusView.Show(rightMonster);
        }

        // ログ欄を空にする
        if (actionLogView != null)
        {
            actionLogView.Clear();
        }
    }

    /// <summary>
    /// 戦闘を実行してログを順番に表示する
    /// </summary>
    private async UniTask PlayBattleAsync()
    {
        GameSession session = GameSession.Instance;

        // 賭け金をここで消費する
        // 外した場合はこのまま没収になる
        bool consumeSuccess = session.TryConsumeCoin(session.CurrentBet.Amount);

        // コインが足りなければBetへ戻す
        if (!consumeSuccess)
        {
            SceneManager.LoadScene(SceneNames.Bet);
            return;
        }

        // 消費後のコイン表示を更新
        coinText.text = $"nokorikakekin:{session.CurrentCoin}";

        // 開始ログ
        await actionLogView.AppendLineAnimated("闇の闘技場、開幕...");
        await UniTask.Delay(500);

        // BattleSimulator で戦闘結果を計算する
        MatchResultData result = battleSimulator.Simulate(
            session.LeftMonster,
            session.RightMonster,
            session.CurrentBet
        );

        // 計算済みログを順番に流す
        foreach (string log in result.BattleLogs)
        {
            await actionLogView.AppendLineAnimated(log);
            await UniTask.Delay(300);

            // 現在HPを反映し直す
            // Simulate後は最終状態になっているので、簡易的に最終HPを反映する形
            if (leftStatusView != null)
            {
                leftStatusView.UpdateHp(session.LeftMonster);
            }

            if (rightStatusView != null)
            {
                rightStatusView.UpdateHp(session.RightMonster);
            }
        }

        // 的中した場合は報酬コインを加算する
        if (result.IsPredictionSuccess)
        {
            session.AddCoin(result.RewardCoin);
            coinText.text = $"nokorikakekin:{session.CurrentCoin}";
            await actionLogView.AppendLineAnimated($"予想的中！ {result.RewardCoin} コイン獲得！");
        }
        else
        {
            await actionLogView.AppendLineAnimated("予想失敗……掛け金は没収された。");
        }

        // 今回の結果を保存する
        session.SetMatchResult(result);

        // 少し待ってからResultへ移動する
        await UniTask.Delay(1000);
        SceneManager.LoadScene(SceneNames.Result);
    }
}