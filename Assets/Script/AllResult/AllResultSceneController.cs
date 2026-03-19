using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 最終結果画面を管理するクラス
/// 主な役割:
/// ・最終コイン数の表示
/// ・エンディング表示
/// ・タイトルへ戻る
/// </summary>
public class AllResultSceneController : MonoBehaviour
{
    [Header("最終結果表示UI")]
    [SerializeField] private TMP_Text finalCoinText;
    [SerializeField] private TMP_Text endingText;

    /// <summary>
    /// シーン開始時に呼ばれる
    /// 最終結果を表示する
    /// </summary>
    private void Start()
    {
        // セッションが無ければタイトルへ戻す
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // 最終結果を表示する
        SetupFinalResultView();
    }

    public void Update()
    {
        OnClickBackToTitle();
    }

    /// <summary>
    /// 最終コイン数とエンディング文を表示する
    /// </summary>
    private void SetupFinalResultView()
    {
        int coin = GameSession.Instance.CurrentCoin;

        // 最終コイン表示
        finalCoinText.text = $"{coin}";

        // コイン数によってエンディング分岐
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
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        SceneManager.LoadScene(SceneNames.Title);
    }
}