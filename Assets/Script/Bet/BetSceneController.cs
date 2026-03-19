using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Betシーン全体を管理するクラス
/// ・コイン表示
/// ・左右キャラ表示
/// ・左右それぞれの詳細ステータス画面表示
/// ・賭け金調整
/// ・左右どちらに賭けるか選択
/// ・Battleシーンへの移動
/// を担当する
/// </summary>
public class BetSceneController : MonoBehaviour
{
    [Header("上部表示")]
    [SerializeField] private TMP_Text coinText;

    [Header("左側UI")]
    [SerializeField] private TMP_Text leftCharaNameText;
    [SerializeField] private Image leftCharaImage;
    [SerializeField] private Button leftStatusButton;
    [SerializeField] private Button leftSelectButton;

    [Header("右側UI")]
    [SerializeField] private TMP_Text rightCharaNameText;
    [SerializeField] private Image rightCharaImage;
    [SerializeField] private Button rightStatusButton;
    [SerializeField] private Button rightSelectButton;

    [Header("通常の選択画面をまとめた親")]
    [SerializeField] private GameObject selectedUIRoot;

    [Header("左の詳細ステータス画面")]
    [SerializeField] private BetStatusDetailView leftStatusView;

    [Header("右の詳細ステータス画面")]
    [SerializeField] private BetStatusDetailView rightStatusView;

    [Header("賭け金UI")]
    [SerializeField] private TMP_Text betAmountText;
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;

    // 現在どちらを選んだか
    private PredictionSide selectedSide;

    // まだ未選択かどうか
    private bool hasSelectedSide = false;

    // 現在の賭け金
    private int currentBetAmount = 10;

    // 最小 / 最大 / 増減量
    private const int MIN_BET = 10;
    private const int BET_STEP = 1;

    /// <summary>
    /// シーン開始時に呼ばれる
    /// </summary>
    private void Start()
    {
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        RefreshCoinText();
        SetupMonsterViews();

        if (leftStatusView != null)
        {
            leftStatusView.Clear();
            leftStatusView.Close();
        }

        if (rightStatusView != null)
        {
            rightStatusView.Clear();
            rightStatusView.Close();
        }

        OpenSelectedUI();

        hasSelectedSide = false;
        UpdateSelectButtonColors();

        // 初期賭け金を設定
        currentBetAmount = Mathf.Min(MIN_BET, GameSession.Instance.CurrentCoin);
        if (currentBetAmount <= 0)
        {
            currentBetAmount = MIN_BET;
        }

        RefreshBetAmountText();
    }

    /// <summary>
    /// 通常選択UIを開く
    /// </summary>
    public void OpenSelectedUI()
    {
        if (selectedUIRoot != null)
        {
            selectedUIRoot.SetActive(true);
            return;
        }

        leftCharaNameText.enabled = true;
        leftCharaImage.enabled = true;
        leftStatusButton.gameObject.SetActive(true);
        leftSelectButton.gameObject.SetActive(true);

        rightCharaNameText.enabled = true;
        rightCharaImage.enabled = true;
        rightStatusButton.gameObject.SetActive(true);
        rightSelectButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// 通常選択UIを閉じる
    /// </summary>
    public void CloseSelectedUI()
    {
        if (selectedUIRoot != null)
        {
            selectedUIRoot.SetActive(false);
            return;
        }

        leftCharaNameText.enabled = false;
        leftCharaImage.enabled = false;
        leftStatusButton.gameObject.SetActive(false);
        leftSelectButton.gameObject.SetActive(false);

        rightCharaNameText.enabled = false;
        rightCharaImage.enabled = false;
        rightStatusButton.gameObject.SetActive(false);
        rightSelectButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 左の詳細ステータスを表示する
    /// </summary>
    public void OnClickLeftStatus()
    {
        if (leftStatusView == null) return;

        CloseSelectedUI();

        if (rightStatusView != null)
        {
            rightStatusView.Close();
        }

        leftStatusView.Open();
        leftStatusView.Show(GameSession.Instance.LeftMonster);
    }

    /// <summary>
    /// 右の詳細ステータスを表示する
    /// </summary>
    public void OnClickRightStatus()
    {
        if (rightStatusView == null) return;

        CloseSelectedUI();

        if (leftStatusView != null)
        {
            leftStatusView.Close();
        }

        rightStatusView.Open();
        rightStatusView.Show(GameSession.Instance.RightMonster);
    }

    /// <summary>
    /// 詳細画面から通常画面へ戻る
    /// </summary>
    public void OnClickBackMenu()
    {
        if (leftStatusView != null)
        {
            leftStatusView.Close();
        }

        if (rightStatusView != null)
        {
            rightStatusView.Close();
        }

        OpenSelectedUI();
    }

    /// <summary>
    /// 賭け金を増やす
    /// </summary>
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

    /// <summary>
    /// 賭け金を減らす
    /// </summary>
    public void OnClickBetDown()
    {
        currentBetAmount -= BET_STEP;
        if (currentBetAmount < MIN_BET)
        {
            currentBetAmount = MIN_BET;
        }

        RefreshBetAmountText();
    }

    /// <summary>
    /// 左に賭ける
    /// </summary>
    public void OnClickLeftSelect()
    {
        selectedSide = PredictionSide.Left;
        hasSelectedSide = true;

        UpdateSelectButtonColors();
        SaveBetData();

        SceneManager.LoadScene(SceneNames.Battle);
    }

    /// <summary>
    /// 右に賭ける
    /// </summary>
    public void OnClickRightSelect()
    {
        selectedSide = PredictionSide.Right;
        hasSelectedSide = true;

        UpdateSelectButtonColors();
        SaveBetData();

        SceneManager.LoadScene(SceneNames.Battle);
    }

    /// <summary>
    /// コイン表示更新
    /// </summary>
    private void RefreshCoinText()
    {
        coinText.text = $"Coin: {GameSession.Instance.CurrentCoin}";
    }

    /// <summary>
    /// 賭け金表示更新
    /// </summary>
    private void RefreshBetAmountText()
    {
        if (betAmountText != null)
        {
            betAmountText.text = currentBetAmount.ToString();
        }
    }

    /// <summary>
    /// 今回賭けられる最大額を返す
    /// </summary>
    private int GetMaxBetAmount()
    {
        // 所持コイン以下、かつ10刻みで扱いやすくする
        int coin = GameSession.Instance.CurrentCoin;

        if (coin < MIN_BET)
        {
            return MIN_BET;
        }

        return (coin / BET_STEP) * BET_STEP;
    }

    /// <summary>
    /// 左右モンスターの名前と画像を表示する
    /// </summary>
    private void SetupMonsterViews()
    {
        MonsterData leftMonster = GameSession.Instance.LeftMonster;
        MonsterData rightMonster = GameSession.Instance.RightMonster;

        leftCharaNameText.text = leftMonster.Name;
        if (leftCharaImage != null)
        {
            leftCharaImage.sprite = leftMonster.Icon;
        }

        rightCharaNameText.text = rightMonster.Name;
        if (rightCharaImage != null)
        {
            rightCharaImage.sprite = rightMonster.Icon;
        }
    }

    /// <summary>
    /// Selectボタンの色を更新する
    /// </summary>
    private void UpdateSelectButtonColors()
    {
        if (leftSelectButton == null || rightSelectButton == null) return;

        Color normalColor = Color.white;
        Color selectedColor = new Color(1f, 0.9f, 0.4f);

        Image leftButtonImage = leftSelectButton.GetComponent<Image>();
        Image rightButtonImage = rightSelectButton.GetComponent<Image>();

        if (leftButtonImage == null || rightButtonImage == null) return;

        if (!hasSelectedSide)
        {
            leftButtonImage.color = normalColor;
            rightButtonImage.color = normalColor;
            return;
        }

        if (selectedSide == PredictionSide.Left)
        {
            leftButtonImage.color = selectedColor;
            rightButtonImage.color = normalColor;
        }
        else
        {
            leftButtonImage.color = normalColor;
            rightButtonImage.color = selectedColor;
        }
    }

    /// <summary>
    /// 賭け情報を保存する
    /// </summary>
    private void SaveBetData()
    {
        if (!hasSelectedSide) return;

        // 左右の強さから倍率を決定する
        float payoutMultiplier = BetOddsCalculator.CalculatePayoutMultiplier(
            GameSession.Instance.LeftMonster,
            GameSession.Instance.RightMonster,
            selectedSide
        );

        BetData betData = new BetData
        {
            Side = selectedSide,
            Amount = currentBetAmount,
            InspectLevel = InspectLevel.Full,
            PayoutMultiplier = payoutMultiplier
        };

        GameSession.Instance.SetBet(betData);
    }
}