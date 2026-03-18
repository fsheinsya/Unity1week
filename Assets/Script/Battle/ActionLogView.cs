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
    /// </summary>
    public void AppendLine(string message)
    {
        // すでに文字があるなら改行してから追加する
        if (!string.IsNullOrEmpty(logText.text))
        {
            logText.text += "\n";
        }

        logText.text += message;
    }

    /// <summary>
    /// 1文字ずつ表示しながら1行追加する
    /// </summary>
    public async UniTask AppendLineAnimated(string message, int delayMillisecondsPerChar = 20)
    {
        // すでに文字があるなら改行してから追加する
        if (!string.IsNullOrEmpty(logText.text))
        {
            logText.text += "\n";
        }

        // 1文字ずつ順番に追加する
        foreach (char c in message)
        {
            logText.text += c;
            await UniTask.Delay(delayMillisecondsPerChar);
        }
    }
}