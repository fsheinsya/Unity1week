using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Betシーン全体を管理するクラス
/// 今のUIに合わせて、以下を担当する
/// ・コイン表示
/// ・左右キャラ名表示
/// ・左右キャラ画像表示
/// ・Status Miru ボタン処理
/// ・Select ボタン処理
/// ・Battle シーンへの移動
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

    //BetStatusViewを参照
    [Header("ステータス表示欄（任意）")]
    [SerializeField] private BetStatusView statusView;

    // 現在どちらに賭けるかを保持する
    private PredictionSide? selectedSide;

    // 最初は固定で 10 コイン賭ける
    private const int BET_AMOUNT = 10;


    /// <summary>
    /// シーン開始時に呼ばれる
    /// 画面へセッション情報を表示する
    /// </summary>
    private void Start()
    {
        // GameSession が存在しない場合は Title に戻す
        if (GameSession.Instance == null)
        {
            SceneManager.LoadScene(SceneNames.Title);
            return;
        }

        // コイン表示を更新する
        RefreshCoinText();

        // 左右モンスター情報をUIに反映する
        SetupMonsterViews();

        // ステータス表示欄がある場合は初期化しておく
        if (statusView != null)
        {
            statusView.Clear();
        }

        // 最初はどちらも選ばれていない状態にする
        UpdateSelectButtonColors();
    }

    /// <summary>
    /// 選択画面をつける
    /// </summary>
    public void OpenSelectedUI()
    {
        //選択画面の表示
        coinText.enabled = true;
        leftCharaNameText.enabled = true;
        leftCharaImage.enabled = true;
        leftSelectButton.enabled = true;
        leftStatusButton.enabled = true;
        rightCharaNameText.enabled = true;
        rightCharaImage.enabled = true;
        rightSelectButton.enabled = true;
        rightStatusButton.enabled = true;
    }

    /// <summary>
    /// 選択画面を消す
    /// </summary>
    public void CloseSelectedUI()
    {
        // コイン表示を非表示にする
        coinText.enabled = false;

        // 左キャラクターUIを非表示
        leftCharaNameText.enabled = false;   // 左キャラ名前
        leftCharaImage.enabled = false;      // 左キャラ画像
        leftSelectButton.enabled = false;    // 左選択ボタン
        leftStatusButton.enabled = false;    // 左ステータスボタン

        // 右キャラクターUIを非表示
        rightCharaNameText.enabled = false;  // 右キャラ名前
        rightCharaImage.enabled = false;     // 右キャラ画像
        rightSelectButton.enabled = false;   // 右選択ボタン
        rightStatusButton.enabled = false;   // 右ステータスボタン
    }

    /// <summary>
    /// ステータス画面をつける
    /// </summary>
    public void OpenStatusUI()
    {
        statusView.hpText.enabled = true;
        statusView.atkText.enabled = true;
        statusView.defText.enabled = true;
        statusView.growthText.enabled = true;
        statusView.abilityText.enabled = true;
    }

    /// <summary>
    /// ステータス画面を消す
    /// </summary>
    public void CloseStatusUI()
    {

    }

    /// <summary>
    /// 左の Status Miru ボタンから呼ばれる
    /// 左モンスターのステータスを表示する
    /// </summary>
    public void OnClickLeftStatus()
    {
        // statusView が設定されていない場合は何もしない
        if (statusView == null) return;

        CloseSelectedUI();
        OpenStatusUI();

        statusView.Show(GameSession.Instance.LeftMonster);
    }

    /// <summary>
    /// 右の Status Miru ボタンから呼ばれる
    /// 右モンスターのステータスを表示する
    /// </summary>
    public void OnClickRightStatus()
    {
        // statusView が設定されていない場合は何もしない
        if (statusView == null) return;

        CloseSelectedUI();
        OpenStatusUI();

        statusView.Show(GameSession.Instance.RightMonster);
    }

    /// <summary>
    /// 左の Select ボタンから呼ばれる
    /// 左に賭ける情報を保存して Battle に進む
    /// </summary>
    public void OnClickLeftSelect()
    {
        // 左側を選択状態にする
        selectedSide = PredictionSide.Left;

        // ボタン色を更新して視覚的に分かるようにする
        UpdateSelectButtonColors();

        // 賭け情報をセッションに保存する
        SaveBetData();

        // Battleシーンへ移動する
        SceneManager.LoadScene(SceneNames.Battle);
    }

    /// <summary>
    /// 右の Select ボタンから呼ばれる
    /// 右に賭ける情報を保存して Battle に進む
    /// </summary>
    public void OnClickRightSelect()
    {
        // 右側を選択状態にする
        selectedSide = PredictionSide.Right;

        // ボタン色を更新して視覚的に分かるようにする
        UpdateSelectButtonColors();

        // 賭け情報をセッションに保存する
        SaveBetData();

        // Battleシーンへ移動する
        SceneManager.LoadScene(SceneNames.Battle);
    }

    /// <summary>
    /// 現在の所持コインを上部へ表示する
    /// </summary>
    private void RefreshCoinText()
    {
        coinText.text = $"Coin: {GameSession.Instance.CurrentCoin}";
    }

    /// <summary>
    /// 左右モンスターの名前と画像を画面に反映する
    /// </summary>
    private void SetupMonsterViews()
    {
        MonsterData leftMonster = GameSession.Instance.LeftMonster;
        MonsterData rightMonster = GameSession.Instance.RightMonster;

        // 左側の名前を表示
        leftCharaNameText.text = leftMonster.Name;

        // 左側の画像を表示
        if (leftCharaImage != null)
        {
            leftCharaImage.sprite = leftMonster.Icon;
        }

        // 右側の名前を表示
        rightCharaNameText.text = rightMonster.Name;

        // 右側の画像を表示
        if (rightCharaImage != null)
        {
            rightCharaImage.sprite = rightMonster.Icon;
        }
    }

    /// <summary>
    /// 現在の選択状態に応じて Select ボタンの色を変える
    /// 選んだ側が少し分かりやすくなる
    /// </summary>
    private void UpdateSelectButtonColors()
    {
        if (leftSelectButton == null || rightSelectButton == null) return;

        // 基本色
        Color normalColor = Color.white;

        // 選択中の色
        Color selectedColor = new Color(1f, 0.9f, 0.4f);

        Image leftButtonImage = leftSelectButton.GetComponent<Image>();
        Image rightButtonImage = rightSelectButton.GetComponent<Image>();

        if (leftButtonImage == null || rightButtonImage == null) return;

        // まだ何も選ばれていないなら両方白
        if (selectedSide == null)
        {
            leftButtonImage.color = normalColor;
            rightButtonImage.color = normalColor;
            return;
        }

        // 左が選ばれていたら左を強調
        if (selectedSide == PredictionSide.Left)
        {
            leftButtonImage.color = selectedColor;
            rightButtonImage.color = normalColor;
        }
        // 右が選ばれていたら右を強調
        else
        {
            leftButtonImage.color = normalColor;
            rightButtonImage.color = selectedColor;
        }
    }

    /// <summary>
    /// 現在の選択内容を GameSession に保存する
    /// 今回は賭け金を 10 固定にしている
    /// </summary>
    private void SaveBetData()
    {
        // 念のため未選択なら保存しない
        if (selectedSide == null) return;

        // 賭け情報を作る
        BetData betData = new BetData
        {
            Side = selectedSide.Value,
            Amount = BET_AMOUNT,

            // 今回は「Status Miru」は情報開示の演出扱いなので、
            // セッション上は Full にしておく
            InspectLevel = InspectLevel.Full
        };

        // セッションに保存する
        GameSession.Instance.SetBet(betData);
    }
}