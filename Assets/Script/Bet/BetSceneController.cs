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
/// ・左ステータスUI / 右ステータスUI の表示切り替え
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

    [Header("ステータス表示欄")]
    [SerializeField] private BetStatusView leftStatusView;
    [SerializeField] private BetStatusView rightStatusView;

    [Header("選択UIをまとめた親（任意）")]
    [SerializeField] private GameObject selectedUIRoot;

    // 現在どちらに賭けるかを保持する
    private PredictionSide selectedSide;

    // まだ選んでいない状態かどうか
    private bool hasSelectedSide = false;

    // 今回は仮で賭け金を固定
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

        // 左右のステータス表示欄を初期化する
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

        // 最初は選択UIを開いておく
        OpenSelectedUI();

        // 最初はどちらも選ばれていない状態にする
        hasSelectedSide = false;

        // ボタン色を初期化する
        UpdateSelectButtonColors();
    }

    /// <summary>
    /// 選択画面を表示する
    /// </summary>
    public void OpenSelectedUI()
    {
        // 親をまとめている場合は親ごと表示
        if (selectedUIRoot != null)
        {
            selectedUIRoot.SetActive(true);
            return;
        }

        // 親を作っていない場合は個別に表示
        leftCharaNameText.enabled = true;
        leftCharaImage.enabled = true;
        leftSelectButton.gameObject.SetActive(true);
        leftStatusButton.gameObject.SetActive(true);

        rightCharaNameText.enabled = true;
        rightCharaImage.enabled = true;
        rightSelectButton.gameObject.SetActive(true);
        rightStatusButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// 選択画面を非表示にする
    /// </summary>
    public void CloseSelectedUI()
    {
        // 親をまとめている場合は親ごと非表示
        if (selectedUIRoot != null)
        {
            selectedUIRoot.SetActive(false);
            return;
        }

        // 親を作っていない場合は個別に非表示
        leftCharaNameText.enabled = false;
        leftCharaImage.enabled = false;
        leftSelectButton.gameObject.SetActive(false);
        leftStatusButton.gameObject.SetActive(false);

        rightCharaNameText.enabled = false;
        rightCharaImage.enabled = false;
        rightSelectButton.gameObject.SetActive(false);
        rightStatusButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 左の Status Miru ボタンから呼ばれる
    /// 左モンスターのステータスを左側UIに表示する
    /// </summary>
    public void OnClickLeftStatus()
    {
        // 左側のステータスUIが設定されていない場合は何もしない
        if (leftStatusView == null) return;

        // 選択画面を閉じる
        CloseSelectedUI();

        // 右側ステータスUIは閉じる
        if (rightStatusView != null)
        {
            rightStatusView.Close();
        }

        // 左側ステータスUIを開いて情報を表示する
        leftStatusView.Open();
        leftStatusView.Show(GameSession.Instance.LeftMonster);
    }

    /// <summary>
    /// 右の Status Miru ボタンから呼ばれる
    /// 右モンスターのステータスを右側UIに表示する
    /// </summary>
    public void OnClickRightStatus()
    {
        // 右側のステータスUIが設定されていない場合は何もしない
        if (rightStatusView == null) return;

        // 選択画面を閉じる
        CloseSelectedUI();

        // 左側ステータスUIは閉じる
        if (leftStatusView != null)
        {
            leftStatusView.Close();
        }

        // 右側ステータスUIを開いて情報を表示する
        rightStatusView.Open();
        rightStatusView.Show(GameSession.Instance.RightMonster);
    }

    /// <summary>
    /// ステータス画面から選択画面へ戻る
    /// Backボタンなどから呼ぶ
    /// </summary>
    public void OnClickBackMenu()
    {
        // 左右のステータスUIを閉じる
        if (leftStatusView != null)
        {
            leftStatusView.Close();
        }

        if (rightStatusView != null)
        {
            rightStatusView.Close();
        }

        // 選択UIを再表示する
        OpenSelectedUI();
    }

    /// <summary>
    /// 左の Select ボタンから呼ばれる
    /// 左に賭ける情報を保存して Battle に進む
    /// </summary>
    public void OnClickLeftSelect()
    {
        // 左側を選択状態にする
        selectedSide = PredictionSide.Left;
        hasSelectedSide = true;

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
        hasSelectedSide = true;

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
        if (!hasSelectedSide)
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
        // まだ未選択なら保存しない
        if (!hasSelectedSide) return;

        // 賭け情報を作る
        BetData betData = new BetData
        {
            Side = selectedSide,
            Amount = BET_AMOUNT,

            // 今回はステータスを見られる想定なので Full にしておく
            InspectLevel = InspectLevel.Full
        };

        // セッションに保存する
        GameSession.Instance.SetBet(betData);
    }
}