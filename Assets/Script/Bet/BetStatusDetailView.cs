using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// ステータス詳細UI（左右キャラ対応版🔥）
/// </summary>
public class BetStatusDetailView : MonoBehaviour
{
    [SerializeField] private GameObject rootObject;

    [Header("キャラ画像")]
    [SerializeField] private Image leftcharaImage;
    [SerializeField] private Image rightcharaImage;

    [Header("能力値（左基準）")]
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text defenseText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text growthText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text evasionText;

    [Header("追加")]
    [SerializeField] private TMP_Text elementText;
    [SerializeField] private TMP_Text oddsText;
    [SerializeField] private TMP_Text personalityText;

    [Header("スキル")]
    [SerializeField] private TMP_Text skillText1;
    [SerializeField] private TMP_Text skillText2;

    [Header("スライド")]
    [SerializeField] private RectTransform statusPanel;
    [SerializeField] private RectTransform elementPanel;
    [SerializeField] private RectTransform oddsPanel;
    [SerializeField] private RectTransform personalityPanel;
    [SerializeField] private RectTransform skillPanel1;
    [SerializeField] private RectTransform skillPanel2;

    private Dictionary<RectTransform, Vector2> originalPos = new();
    private bool hasPlayedAnimation = false;

    private void Awake()
    {
        Cache();
        HideAll();
    }

    private void Cache()
    {
        Save(statusPanel);
        Save(elementPanel);
        Save(oddsPanel);
        Save(personalityPanel);
        Save(skillPanel1);
        Save(skillPanel2);
    }

    private void Save(RectTransform r)
    {
        if (r != null && !originalPos.ContainsKey(r))
            originalPos[r] = r.anchoredPosition;
    }

    private void HideAll()
    {
        Set(statusPanel, false);
        Set(elementPanel, false);
        Set(oddsPanel, false);
        Set(personalityPanel, false);
        Set(skillPanel1, false);
        Set(skillPanel2, false);
    }

    private void Set(RectTransform r, bool v)
    {
        if (r != null) r.gameObject.SetActive(v);
    }

    public void Open()
    {
        rootObject?.SetActive(true);
    }

    public void Close()
    {
        rootObject?.SetActive(false);

        // キャラだけリセット
        leftcharaImage.transform.localScale = Vector3.one;
        rightcharaImage.transform.localScale = Vector3.one;
    }

    // =========================
    // 🔥 表示（左右対応）
    // =========================
    public void Show(MonsterData left, MonsterData right, float odds)
    {
        // 左キャラ
        leftcharaImage.sprite = left.Icon;
        leftcharaImage.enabled = true;
        leftcharaImage.color = Color.white;

        // 右キャラ（反転🔥）
        rightcharaImage.sprite = right.Icon;
        rightcharaImage.enabled = true;
        rightcharaImage.color = Color.white;

        // 🔥 サイズ＆向き
        leftcharaImage.rectTransform.localScale = new Vector3(2f, 2f, 1f);
        rightcharaImage.rectTransform.localScale = new Vector3(2f, 2f, 1f);//こっちは反転しなくていい

        // 能力値（左ベース）
        hpText.text = $"HP:{left.Stats.MaxHp}";
        defenseText.text = $"防御:{left.Stats.Defense}";
        attackText.text = $"攻撃:{left.Stats.Attack}";
        growthText.text = $"成長:{left.Stats.Growth}";
        speedText.text = $"素早さ:{left.Stats.Speed}";
        accuracyText.text = $"命中:{left.Stats.Accuracy}";
        evasionText.text = $"回避:{left.Stats.Evasion}";

        elementText.text = GetElementName(left.Element);
        oddsText.text = $"オッズ:{odds:F1}";
        personalityText.text = GetPersonalityName(left.Personality);

        skillText1.text = GetSkillName(left, 0);
        skillText2.text = GetSkillName(left, 1);
    }

    // =========================
    // 🔥 アニメーション
    // =========================
    public async UniTask PlayOpenAnimation(MonsterData left, MonsterData right, float odds)
    {
        Open();
        Show(left, right, odds);

        // 🔥 2回目以降は何もしない（そのまま表示）
        if (hasPlayedAnimation)
            return;

        hasPlayedAnimation = true;

        // 🔥 初回だけ非表示スタート
        HideAll();

        // キャラポップ
        leftcharaImage.transform.localScale = Vector3.zero;
        rightcharaImage.transform.localScale = Vector3.zero;

        await UniTask.WhenAll(
            leftcharaImage.transform.DOScale(2f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask(),
            rightcharaImage.transform.DOScale(2f, 0.3f).SetEase(Ease.OutBack).AsyncWaitForCompletion().AsUniTask()
        );

        // 初回だけスライド
        await Slide(statusPanel);
        await Slide(elementPanel);
        await Slide(oddsPanel);
        await Slide(personalityPanel);
        await Slide(skillPanel1);
        await Slide(skillPanel2);
    }

    private void ShowAllPanelsInstant()
    {
        Set(statusPanel, true);
        Set(elementPanel, true);
        Set(oddsPanel, true);
        Set(personalityPanel, true);
        Set(skillPanel1, true);
        Set(skillPanel2, true);

        // 🔥 正しい位置に戻す（超重要）
        ResetPosition(statusPanel);
        ResetPosition(elementPanel);
        ResetPosition(oddsPanel);
        ResetPosition(personalityPanel);
        ResetPosition(skillPanel1);
        ResetPosition(skillPanel2);
    }

    private void ResetPosition(RectTransform r)
    {
        if (r != null && originalPos.ContainsKey(r))
        {
            r.anchoredPosition = originalPos[r];
        }
    }

    private async UniTask Slide(RectTransform r)
    {
        if (r == null) return;

        Vector2 target = originalPos[r];

        r.gameObject.SetActive(true);
        r.anchoredPosition = new Vector2(target.x + 800f, target.y);

        await r.DOAnchorPos(target, 0.4f)
            .SetEase(Ease.OutCubic)
            .AsyncWaitForCompletion();

        await UniTask.Delay(30);//30固定
    }

    // =========================
    // データ
    // =========================
    private string GetSkillName(MonsterData data, int i)
    {
        if (data.Skills == null || i >= data.Skills.Count)
            return "スキルなし";

        return data.Skills[i].SkillName;
    }

    private string GetElementName(ElementType e)
    {
        return e switch
        {
            ElementType.Fire => "炎",
            ElementType.Water => "水",
            ElementType.Grass => "草",
            ElementType.Thunder => "雷",
            ElementType.Rock => "岩",
            ElementType.Dark => "闇",
            ElementType.Light => "光",
            _ => "無"
        };
    }

    private string GetPersonalityName(PersonalityType p)
    {
        return p switch
        {
            PersonalityType.Bold => "大胆",
            PersonalityType.Calm => "冷静",
            PersonalityType.Flexible => "柔軟",
            PersonalityType.Taunt => "挑発",
            PersonalityType.Aggressive => "積極的",
            PersonalityType.Timid => "臆病",
            _ => "なし"
        };
    }
}