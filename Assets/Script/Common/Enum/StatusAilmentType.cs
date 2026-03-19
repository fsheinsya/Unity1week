/// <summary>
/// 状態異常の種類
/// </summary>
public enum StatusAilmentType
{
    None,
    Stun,       // 1ターン行動不能
    Poison,     // 毎ターンダメージ
    SkillSeal   // スキル使用不可
}