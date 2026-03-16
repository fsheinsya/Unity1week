using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 戦闘ログ欄の表示を担当するクラス
/// 主な役割:
/// ・ログの初期化
/// ・1行追加
/// ・文字送り表示
/// </summary>
public class ActionLogView : MonoBehaviour
{
    [Header("ログ表示用テキスト")]
    [SerializeField] private TMP_Text logText;

    /// <summary>
    /// ログ欄を空にする
    /// </summary>
    public void Clear()
    {
        logText.text = "";
    }

    /// <summary>
    /// ログ欄に1行そのまま追加する
    /// </summary>
    public void AppendLine(string message)
    {
        // すでに文字がある場合は改行してから追記する
        if (!string.IsNullOrEmpty(logText.text))
        {
            logText.text += "\n";
        }

        logText.text += message;
    }

    /// <summary>
    /// 文字送り風に1文字ずつログを追加する
    /// </summary>
    public async UniTask AppendLineAnimated(string message, int delayMillisecondsPerChar = 20)
    {
        // すでに文字がある場合は改行してから始める
        if (!string.IsNullOrEmpty(logText.text))
        {
            logText.text += "\n";
        }

        // 1文字ずつ追加していく
        foreach (char c in message)
        {
            logText.text += c;
            await UniTask.Delay(delayMillisecondsPerChar);
        }
    }
}