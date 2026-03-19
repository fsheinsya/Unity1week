using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Titleシーン全体の進行を管理するクラス
/// 主な役割は「ゲーム開始ボタンが押されたら新しいゲームを始めて Bet シーンへ移動すること」
/// </summary>
public class TitleSceneController : MonoBehaviour
{
    //画面をクリックをしたら
    void Update()
    {
      if(Input.GetMouseButtonDown(0)　|| Input.GetMouseButtonDown(1))
        {
            OnClickStartGame();
        }
    }

    /// <summary>
    /// Startボタンから呼ばれるメソッド
    /// ゲーム状態を初期化して、Betシーンへ移動する
    /// </summary>
    public void OnClickStartGame()
    {
        // GameSession に新規ゲーム開始を依頼する
        // ここでコイン数・ラウンド数・対戦カードなどが初期化される
        GameSession.Instance.StartNewGame();

        // 賭け画面へ移動する
        SceneManager.LoadScene(SceneNames.Bet);
    }
}
