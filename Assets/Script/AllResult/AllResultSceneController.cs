using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全3試合終了後の最終結果画面を管理するクラス
/// 主な役割:
/// ・最終所持コインを表示
/// ・エンディング文を分岐表示
/// ・タイトルへ戻る
/// </summary>
public class AllResultSceneController : MonoBehaviour
{
    [Header("最終結果表示UI")]
    [SerializeField] private TMP_Text finalCoinText;
    [SerializeField] private TMP_Text endingText;

    /// <summary>
    /// シーン開始時に最終コイン数とエンディングを表示する
    /// </summary>
    private void Start()
    {
        GameSession session = GameSession.Instance;

        // セッションがない場合は安全のため Title に戻す
        if (session == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        int coin = session.CurrentCoin;

        // 最終コイン数を表示
        finalCoinText.text = $"Final Coin : {coin}";

        // 所持コイン数に応じてエンディングを分岐する
        if (coin <= 0)
        {
            endingText.text = "破滅エンド\n闇闘技場にすべてを奪われた。";
        }
        else if (coin <= 80)
        {
            endingText.text = "敗北エンド\n生き残ったが、何も残らなかった。";
        }
        else if (coin <= 180)
        {
            endingText.text = "凡人エンド\n少し勝ったが、伝説にはなれなかった。";
        }
        else
        {
            endingText.text = "覇者エンド\nあなたは闇闘技場の勝者となった。";
        }
    }

    /// <summary>
    /// タイトルへ戻るボタンから呼ばれる
    /// </summary>
    public void OnClickBackToTitle()
    {
        SceneManager.LoadScene(SceneNames.Title);
    }
}