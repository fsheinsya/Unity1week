using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Betシーン全体を管理するクラス
/// ・コイン表示
/// ・左右キャラ表示
/// ・左右それぞれの詳細ステータス画面表示
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

    // 現在どちらを選んだか
    private PredictionSide selectedSide;

    // まだ未選択かどうか
    private bool hasSelectedSide = false;

    // 仮で固定賭け金
    private const int BET_AMOUNT = 10;

    /// <summary>
    /// シーン開始時に呼ばれる
    /// </summary>
    private void Start()
    {
        // セッションが無ければタイトルへ戻す
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // コイン表示更新
        RefreshCoinText();

        // 左右キャラ表示更新
        SetupMonsterViews();

        // 左右の詳細画面を初期化して閉じる
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

        // 通常選択画面を開く
        OpenSelectedUI();

        // 選択状態初期化
        hasSelectedSide = false;
        UpdateSelectButtonColors();
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
        // 左側ビューが無ければ何もしない
        if (leftStatusView == null) return;

        // 通常選択画面を閉じる
        CloseSelectedUI();

        // 右側の詳細画面は閉じる
        if (rightStatusView != null)
        {
            rightStatusView.Close();
        }

        // 左側の詳細画面を開いて、左モンスター情報を表示する
        leftStatusView.Open();
        leftStatusView.Show(GameSession.Instance.LeftMonster);
    }

    /// <summary>
    /// 右の詳細ステータスを表示する
    /// </summary>
    public void OnClickRightStatus()
    {
        // 右側ビューが無ければ何もしない
        if (rightStatusView == null) return;

        // 通常選択画面を閉じる
        CloseSelectedUI();

        // 左側の詳細画面は閉じる
        if (leftStatusView != null)
        {
            leftStatusView.Close();
        }

        // 右側の詳細画面を開いて、右モンスター情報を表示する
        rightStatusView.Open();
        rightStatusView.Show(GameSession.Instance.RightMonster);
    }

    /// <summary>
    /// 詳細画面から通常画面へ戻る
    /// 左右どちらのBackボタンから押されてもよい
    /// </summary>
    public void OnClickBackMenu()
    {
        // 左右両方の詳細画面を閉じる
        if (leftStatusView != null)
        {
            leftStatusView.Close();
        }

        if (rightStatusView != null)
        {
            rightStatusView.Close();
        }

        // 通常選択画面を再表示
        OpenSelectedUI();
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
    /// 左右モンスターの名前と画像を表示する
    /// </summary>
    private void SetupMonsterViews()
    {
        MonsterData leftMonster = GameSession.Instance.LeftMonster;
        MonsterData rightMonster = GameSession.Instance.RightMonster;

        // 左側表示
        leftCharaNameText.text = leftMonster.Name;
        if (leftCharaImage != null)
        {
            leftCharaImage.sprite = leftMonster.Icon;
        }

        // 右側表示
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

        // 通常色
        Color normalColor = Color.white;

        // 選択中色
        Color selectedColor = new Color(1f, 0.9f, 0.4f);

        Image leftButtonImage = leftSelectButton.GetComponent<Image>();
        Image rightButtonImage = rightSelectButton.GetComponent<Image>();

        if (leftButtonImage == null || rightButtonImage == null) return;

        // 未選択なら両方通常色
        if (!hasSelectedSide)
        {
            leftButtonImage.color = normalColor;
            rightButtonImage.color = normalColor;
            return;
        }

        // 選択側だけ強調する
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

        BetData betData = new BetData
        {
            Side = selectedSide,
            Amount = BET_AMOUNT,
            InspectLevel = InspectLevel.Full
        };

        GameSession.Instance.SetBet(betData);
    }
}