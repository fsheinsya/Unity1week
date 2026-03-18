using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Betシーンで、1体分のモンスター情報を表示するクラス
/// LeftStatusUI と RightStatusUI の両方に付けて使う
/// </summary>
public class BetStatusView : MonoBehaviour
{
    [Header("表示全体をまとめる親")]
    [SerializeField] public GameObject rootObject;

    [Header("キャラクター画像")]
    [SerializeField] public Image charaImage;

    [Header("ステータステキスト")]
    [SerializeField] public TMP_Text hpText;
    [SerializeField] public TMP_Text atkText;
    [SerializeField] public TMP_Text defText;
    [SerializeField] public TMP_Text growthText;
    [SerializeField] public TMP_Text abilityText;

    /// <summary>
    /// 最初に呼んで、表示を空状態にする
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
        if (charaImage != null)
        {
            charaImage.sprite = data.Icon;
        }

        hpText.text = $"HP : {data.Stats.MaxHp}";
        atkText.text = $"ATK : {data.Stats.Attack}";
        defText.text = $"DEF : {data.Stats.Defense}";
        growthText.text = $"GW : {data.Stats.Growth}";
        abilityText.text = $"Ability : {GetAbilityName(data.SpecialAbility)}";
    }

    /// <summary>
    /// ステータスUI全体を表示する
    /// </summary>
    public void Open()
    {
        if (rootObject != null)
        {
            rootObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// ステータスUI全体を非表示にする
    /// </summary>
    public void Close()
    {
        if (rootObject != null)
        {
            rootObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// enum の特殊能力を表示用文字列に変換する
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