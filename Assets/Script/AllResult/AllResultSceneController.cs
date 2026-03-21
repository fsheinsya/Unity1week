using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class AllResultSceneController : MonoBehaviour
{
    [Header("最終結果表示UI")]
    [SerializeField] private TMP_Text finalCoinText;
    [SerializeField] private TMP_Text endingText;

    private bool isAnimating = false;

    private async void Start()
    {
        // セッションチェック
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // 演出開始
        await PlayResultAnimation();
    }

    private void Update()
    {
        // アニメ中はクリック無効
        if (isAnimating) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene(SceneNames.Title);
        }
    }

    /// <summary>
    /// 結果演出メイン
    /// </summary>
    private async UniTask PlayResultAnimation()
    {
        isAnimating = true;

        int coin = GameSession.Instance.CurrentCoin;

        //-------------------------
        // ① コインカウントアップ
        //-------------------------
        finalCoinText.text = "0";

        int displayCoin = 0;

        await DOTween.To(() => displayCoin, x =>
        {
            displayCoin = x;
            finalCoinText.text = displayCoin.ToString();
        }, coin, 1.5f) // ←時間調整OK
        .SetEase(Ease.OutCubic)
        .AsyncWaitForCompletion();

        await UniTask.Delay(300);

        //-------------------------
        // ② エンディングテキスト決定
        //-------------------------
        string ending = "";

        if (coin <= 0)
            ending = "破滅エンド\n闇闘技場にすべてを奪われた。";
        else if (coin <= 100)
            ending = "敗北エンド\n生き残ったが、何も残らなかった。";
        else if (coin <= 200)
            ending = "凡人エンド\n少し勝ったが、伝説にはなれなかった。";
        else
            ending = "覇者エンド\nあなたは闇闘技場の勝者となった。";

        //-------------------------
        // ③ タイピング演出
        //-------------------------
        endingText.text = "";

        foreach (char c in ending)
        {
            endingText.text += c;
            await UniTask.Delay(30); // ←速度調整
        }

        //-------------------------
        // ④ フェードイン（追加演出）
        //-------------------------
        endingText.alpha = 0;
        await endingText.DOFade(1f, 0.5f)
            .AsyncWaitForCompletion(); ;

        isAnimating = false;
    }
}