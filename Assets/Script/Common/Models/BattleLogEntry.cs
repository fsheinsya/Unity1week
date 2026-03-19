using System;

/// <summary>
/// 戦闘ログ1件分のデータ
/// その時点のメッセージと、左右HPの状態を持つ
/// </summary>
[Serializable]
public class BattleLogEntry
{
    // 画面に表示するログ文章
    public string Message;

    // このログ時点での左モンスターHP
    public int LeftHp;

    // このログ時点での右モンスターHP
    public int RightHp;
}