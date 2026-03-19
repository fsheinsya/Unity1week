using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 戦闘ログ欄の表示を担当するクラス
/// 下の大きいテキストウィンドウにログを出す
/// </summary>
public class ActionLogView : MonoBehaviour
{
    [Header("ログ表示用テキスト")]
    [SerializeField] private TMP_Text logText;

    /// <summary>
    /// ログを空にする
    /// </summary>
    public void Clear()
    {
        logText.text = "";
    }

    /// <summary>
    /// 1行そのまま追加する
    /// 必要なら残して使える
    /// </summary>
    public void AppendLine(string message)
    {
        if (!string.IsNullOrEmpty(logText.text))
        {
            logText.text += "\n";
        }

        logText.text += message;
    }

    /// <summary>
    /// 1文字ずつ表示しながら1文だけ表示する
    /// 毎回テキストを消してから新しい文を出す
    /// </summary>
    public async UniTask ShowMessageAnimated(string message, int delayMillisecondsPerChar = 20)
    {
        // 前の文章を消す
        logText.text = "";

        // 1文字ずつ表示する
        foreach (char c in message)
        {
            logText.text += c;
            await UniTask.Delay(delayMillisecondsPerChar);
        }
    }

    /// <summary>
    /// 文字送りなしで1文だけ表示する
    /// </summary>
    public void ShowMessage(string message)
    {
        logText.text = message;
    }
}