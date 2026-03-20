using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Betシーン管理（長押し対応版🔥）
/// </summary>
public class BetSceneController : MonoBehaviour
{
    private enum UIState
    {
        Select,
        Status
    }

    private UIState currentState;

    [Header("上部")]
    [SerializeField] private TMP_Text coinText;

    [Header("左")]
    [SerializeField] private TMP_Text leftName;
    [SerializeField] private Image leftImage;
    [SerializeField] private Button leftStatusBtn;
    [SerializeField] private Button leftSelectBtn;

    [Header("右")]
    [SerializeField] private TMP_Text rightName;
    [SerializeField] private Image rightImage;
    [SerializeField] private Button rightStatusBtn;
    [SerializeField] private Button rightSelectBtn;

    [Header("ステータス")]
    [SerializeField] private BetStatusDetailView leftStatusView;
    [SerializeField] private BetStatusDetailView rightStatusView;

    [Header("Root")]
    [SerializeField] private GameObject selectRoot;
    [SerializeField] private GameObject betRoot;

    [Header("フェード")]
    [SerializeField] private CanvasGroup fadePanel;

    [Header("賭け金")]
    [SerializeField] private TMP_Text betAmountText;

    private PredictionSide selectedSide;
    private bool isTransitioning = false;

    // =========================
    // 賭け金設定
    // =========================

    private int currentBetAmount = 10;
    private const int MIN_BET = 10;
    private const int BET_STEP = 1;

    // =========================
    // 長押し制御🔥
    // =========================
    private bool isPressing = false;
    private bool isIncrease = true;
    private CancellationTokenSource cts;

    // =========================
    // 初期化
    // =========================
    private void Start()
    {
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        Setup();
        ChangeState(UIState.Select);
        RefreshBetAmountText();

        PlayIntro().Forget();
    }

    private void Setup()
    {
        coinText.text = $"Coin: {GameSession.Instance.CurrentCoin}";

        var left = GameSession.Instance.LeftMonster;
        var right = GameSession.Instance.RightMonster;

        leftName.text = left.Name;
        leftImage.sprite = left.Icon;

        rightName.text = right.Name;
        rightImage.sprite = right.Icon;
    }

    private void ChangeState(UIState state)
    {
        currentState = state;

        if (state == UIState.Select)
        {
            selectRoot.SetActive(true);
            betRoot.SetActive(true);

            leftStatusView?.Close();
            rightStatusView?.Close();
        }
        else
        {
            selectRoot.SetActive(false);
            betRoot.SetActive(false);
        }
    }

    // =========================
    // UI演出
    // =========================
    private async UniTask PlayIntro()
    {
        coinText.alpha = 0f;
        await coinText.DOFade(1f, 0.5f).AsyncWaitForCompletion();

        var l = leftImage.rectTransform;
        var r = rightImage.rectTransform;

        float lx = l.anchoredPosition.x;
        float rx = r.anchoredPosition.x;

        l.anchoredPosition = new Vector2(-800, l.anchoredPosition.y);
        r.anchoredPosition = new Vector2(800, r.anchoredPosition.y);

        await UniTask.WhenAll(
            l.DOAnchorPosX(lx, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask(),
            r.DOAnchorPosX(rx, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask()
        );
    }

    // =========================
    // ステータス表示
    // =========================
    public void OnClickLeftStatus()
    {
        ChangeState(UIState.Status);

        float odds = BetOddsCalculator.CalculatePayoutMultiplier(
            GameSession.Instance.LeftMonster,
            GameSession.Instance.RightMonster,
            PredictionSide.Left
        );

        leftStatusView.PlayOpenAnimation(GameSession.Instance.LeftMonster, GameSession.Instance.RightMonster, odds).Forget();
    }

    public void OnClickRightStatus()
    {
        ChangeState(UIState.Status);

        float odds = BetOddsCalculator.CalculatePayoutMultiplier(
            GameSession.Instance.LeftMonster,
            GameSession.Instance.RightMonster,
            PredictionSide.Right
        );

        rightStatusView.PlayOpenAnimation(GameSession.Instance.LeftMonster, GameSession.Instance.RightMonster, odds).Forget();
    }

    public void OnClickBackMenu()
    {
        ChangeState(UIState.Select);
    }

    // =========================
    // バトル遷移
    // =========================
    public void OnClickLeftSelect()
    {
        selectedSide = PredictionSide.Left;
        SaveBet();
        GoBattle().Forget();
    }

    public void OnClickRightSelect()
    {
        selectedSide = PredictionSide.Right;
        SaveBet();
        GoBattle().Forget();
    }

    private async UniTask GoBattle()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        await fadePanel.DOFade(1f, 0.5f).AsyncWaitForCompletion();
        SceneManager.LoadScene(SceneNames.Battle);
    }

    private void SaveBet()
    {
        float multi = BetOddsCalculator.CalculatePayoutMultiplier(
            GameSession.Instance.LeftMonster,
            GameSession.Instance.RightMonster,
            selectedSide
        );

        GameSession.Instance.SetBet(new BetData
        {
            Side = selectedSide,
            Amount = currentBetAmount, // ★修正：賭け金反映
            PayoutMultiplier = multi
        });
    }

    // =========================
    // 賭け金操作
    // =========================
    public void OnClickBetUp()
    {
        int maxBet = GetMaxBetAmount();

        currentBetAmount += BET_STEP;
        if (currentBetAmount > maxBet)
        {
            currentBetAmount = maxBet;
        }

        RefreshBetAmountText();
    }

    public void OnClickBetDown()
    {
        currentBetAmount -= BET_STEP;
        if (currentBetAmount < MIN_BET)
        {
            currentBetAmount = MIN_BET;
        }

        RefreshBetAmountText();
    }

    private void RefreshBetAmountText()
    {
        if (betAmountText != null)
        {
            betAmountText.text = currentBetAmount.ToString();
        }
    }

    private int GetMaxBetAmount()
    {
        int coin = GameSession.Instance.CurrentCoin;

        if (coin < MIN_BET)
        {
            return MIN_BET;
        }

        return (coin / BET_STEP) * BET_STEP;
    }

    // =========================
    // 長押し処理🔥
    // =========================

    /// <summary>
    /// ＋ボタン押した
    /// </summary>
    public void StartIncrease()
    {
        isIncrease = true;
        StartLongPress();
    }

    /// <summary>
    /// −ボタン押した
    /// </summary>
    public void StartDecrease()
    {
        isIncrease = false;
        StartLongPress();
    }

    private void StartLongPress()
    {
        isPressing = true;

        cts?.Cancel();
        cts = new CancellationTokenSource();

        HandleLongPress(cts.Token).Forget();
    }

    /// <summary>
    /// ボタン離した
    /// </summary>
    public void StopLongPress()
    {
        isPressing = false;
        cts?.Cancel();
    }

    private async UniTaskVoid HandleLongPress(CancellationToken token)
    {
        // 最初の1回
        Execute();

        await UniTask.Delay(300, cancellationToken: token);

        float interval = 0.2f;

        while (isPressing && !token.IsCancellationRequested)
        {
            Execute();

            // 加速🔥
            interval = Mathf.Max(0.05f, interval - 0.02f);

            await UniTask.Delay((int)(interval * 1000), cancellationToken: token);
        }
    }

    private void Execute()
    {
        if (isIncrease)
        {
            OnClickBetUp();
        }
        else
        {
            OnClickBetDown();
        }
    }
}