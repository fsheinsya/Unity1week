using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Betシーンで、選んだモンスターのステータスを表示するためのクラス
/// StatusUI にアタッチして使う
/// </summary>
public class BetStatusView : MonoBehaviour
{
    //キャラの画像を表示
    [Header("キャラクター画像")]
    [SerializeField] public Image charaImage;

    //キャラのステータスを表示
    [Header("ステータステキスト")]
    [SerializeField] public TMP_Text hpText;
    [SerializeField] public TMP_Text atkText;
    [SerializeField] public TMP_Text defText;
    [SerializeField] public TMP_Text growthText;
    [SerializeField] public TMP_Text abilityText;

    /// <summary>
    /// 最初に呼んで、表示を空っぽにする
    /// </summary>
    public void Clear()
    {
        hpText.text = "HP : ----";
        atkText.text = "ATK : ----";
        defText.text = "DEF : ----";
        growthText.text = "GW : ----";
        abilityText.text = "Ability : ----";
    }

    /// <summary>
    /// 指定したモンスターの情報を表示する
    /// </summary>
    public void Show(MonsterData data)
    {
        hpText.text = $"HP : {data.Stats.MaxHp}";
        atkText.text = $"ATK : {data.Stats.Attack}";
        defText.text = $"DEF : {data.Stats.Defense}";
        growthText.text = $"GW : {data.Stats.Growth}";
        abilityText.text = $"Ability : {GetAbilityName(data.SpecialAbility)}";
    }

    /// <summary>
    /// enum の特殊能力名を、画面表示用の文字列に変換する
    /// </summary>
    private string GetAbilityName(SpecialAbilityType type)
    {
        switch (type)
        {
            case SpecialAbilityType.None:
                return "None";
            case SpecialAbilityType.PowerStrike:
                return "PowerStrike";
            case SpecialAbilityType.IronWall:
                return "IronWall";
            case SpecialAbilityType.Quick:
                return "Quick";
            case SpecialAbilityType.Regenerate:
                return "Regenerate";
            case SpecialAbilityType.Berserk:
                return "Berserk";
            case SpecialAbilityType.Lucky:
                return "Lucky";
            default:
                return "Unknown";
        }
    }
}