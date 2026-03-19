using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Betシーンの詳細ステータス画面を表示するクラス
/// ・キャラ画像
/// ・基本能力値
/// ・性格
/// ・スキル一覧
/// を表示する
/// </summary>
public class BetStatusDetailView : MonoBehaviour
{
    [Header("画面全体をまとめる親")]
    [SerializeField] private GameObject rootObject;

    [Header("左のキャラ画像")]
    [SerializeField] private Image charaImage;

    [Header("能力値表示")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text growthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text evasionText;

    [Header("性格表示")]
    [SerializeField] private TMP_Text personalityText;

    [Header("スキル表示")]
    [SerializeField] private TMP_Text skillText1;
    [SerializeField] private TMP_Text skillText2;

    /// <summary>
    /// 詳細画面を表示する
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
    /// 詳細画面を閉じる
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
    /// 画面表示を初期化する
    /// </summary>
    public void Clear()
    {
        if (hpText != null) hpText.text = "HP:----";
        if (defenseText != null) defenseText.text = "防御力:----";
        if (attackText != null) attackText.text = "攻撃力:----";
        if (growthText != null) growthText.text = "成長率:----";
        if (speedText != null) speedText.text = "素早さ:----";
        if (accuracyText != null) accuracyText.text = "命中率:----";
        if (evasionText != null) evasionText.text = "回避率:----";
        if (personalityText != null) personalityText.text = "なし";
        if (skillText1 != null) skillText1.text = "スキル";
        if (skillText2 != null) skillText2.text = "スキル";
    }

    /// <summary>
    /// 指定したモンスターの詳細情報を表示する
    /// </summary>
    public void Show(MonsterData data)
    {
        // キャラ画像
        if (charaImage != null)
        {
            charaImage.sprite = data.Icon;
        }

        // 能力値
        if (hpText != null) hpText.text = $"HP:{data.Stats.MaxHp}";
        if (defenseText != null) defenseText.text = $"防御力:{data.Stats.Defense}";
        if (attackText != null) attackText.text = $"攻撃力:{data.Stats.Attack}";
        if (growthText != null) growthText.text = $"成長率:{data.Stats.Growth}";
        if (speedText != null) speedText.text = $"素早さ:{data.Stats.Speed}";
        if (accuracyText != null) accuracyText.text = $"命中率:{data.Stats.Accuracy}";
        if (evasionText != null) evasionText.text = $"回避率:{data.Stats.Evasion}";

        // 性格
        if (personalityText != null)
        {
            personalityText.text = GetPersonalityName(data.Personality);
        }

        // スキル
        if (skillText1 != null)
        {
            skillText1.text = GetSkillName(data, 0);
        }

        if (skillText2 != null)
        {
            skillText2.text = GetSkillName(data, 1);
        }
    }

    /// <summary>
    /// 指定インデックスのスキル名を返す
    /// </summary>
    private string GetSkillName(MonsterData data, int index)
    {
        if (data.Skills == null) return "スキルなし";
        if (index < 0 || index >= data.Skills.Count) return "スキルなし";

        return data.Skills[index].SkillName;
    }

    /// <summary>
    /// 性格の日本語名を返す
    /// </summary>
    private string GetPersonalityName(PersonalityType type)
    {
        switch (type)
        {
            case PersonalityType.Bold: return "大胆";
            case PersonalityType.Calm: return "冷静";
            case PersonalityType.Flexible: return "柔軟";
            case PersonalityType.Taunt: return "挑発";
            case PersonalityType.Aggressive: return "積極的";
            case PersonalityType.Timid: return "臆病";
            default: return "なし";
        }
    }
}