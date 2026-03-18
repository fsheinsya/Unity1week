using TMPro;
using UnityEngine;

/// <summary>
/// Battleシーンで、1体分のステータスを表示するクラス
/// 左右のステータス欄それぞれに付けて使う
/// </summary>
public class BattleStatusView : MonoBehaviour
{
    [Header("ステータステキスト")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;

    /// <summary>
    /// 指定したモンスター情報を表示する
    /// </summary>
    public void Show(MonsterData data)
    {
        // 画面に現在の能力値を表示する
        hpText.text = $"HP:{data.CurrentHp}";
        atkText.text = $"ATK:{data.Stats.Attack}";
        defText.text = $"DEF:{data.Stats.Defense}";
    }

    /// <summary>
    /// 現在HPだけを更新する
    /// 戦闘中にHPが減る演出で使いやすい
    /// </summary>
    public void UpdateHp(MonsterData data)
    {
        hpText.text = $"HP:{data.CurrentHp}";
    }
}