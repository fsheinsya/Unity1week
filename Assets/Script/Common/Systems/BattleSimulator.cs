using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトル計算本体
/// 追加パラメータや性格、スキルを使って戦闘する
/// </summary>
public class BattleSimulator
{
    /// <summary>
    /// 戦闘実行
    /// </summary>
    public MatchResultData Simulate(MonsterData left, MonsterData right, BetData betData)
    {
        // ログ一覧
        var logs = new List<BattleLogEntry>();

        // 戦闘開始前にHPを戻す
        left.CurrentHp = left.Stats.MaxHp;
        right.CurrentHp = right.Stats.MaxHp;

        // 状態異常を初期化
        left.StatusAilment = StatusAilmentType.None;
        right.StatusAilment = StatusAilmentType.None;

        BattleSceneController battle = new BattleSceneController();

        AddLog(logs, $"{left.Name} と {right.Name} の戦闘開始！", left, right);

        int turnCount = 1;

        while (left.CurrentHp > 0 && right.CurrentHp > 0)
        {
            AddLog(logs, $"--- Turn {turnCount} ---", left, right);

            // 先攻決定
            bool leftFirst = IsLeftFirst(left, right);

            if (leftFirst)
            {
                ProcessAction(left, right, logs, left, right, turnCount);
                if (right.CurrentHp <= 0) break;

                ProcessAction(right, left, logs, left, right, turnCount);
            }
            else
            {
                ProcessAction(right, left, logs, left, right, turnCount);
                if (left.CurrentHp <= 0) break;

                ProcessAction(left, right, logs, left, right, turnCount);
            }

            // ターン終了効果
            ApplyEndTurnEffect(left, logs, left, right);
            ApplyEndTurnEffect(right, logs, left, right);

            // 1ターン無敵は1ターンで切る
            left.IsInvincible = false;
            right.IsInvincible = false;

            turnCount++;
        }

        PredictionSide winnerSide = left.CurrentHp > 0 ? PredictionSide.Left : PredictionSide.Right;
        string winnerName = left.CurrentHp > 0 ? left.Name : right.Name;

        bool isPredictionSuccess = betData != null && betData.Side == winnerSide;
        int reward = isPredictionSuccess ? betData.Amount * 2 : 0;

        AddLog(logs, $"{winnerName} の勝利！", left, right);

        return new MatchResultData
        {
            WinnerName = winnerName,
            WinnerSide = winnerSide,
            IsPredictionSuccess = isPredictionSuccess,
            RewardCoin = reward,
            BattleLogs = logs
        };
    }

    /// <summary>
    /// 素早さと特殊能力で先攻判定
    /// </summary>
    private bool IsLeftFirst(MonsterData left, MonsterData right)
    {
        int leftSpeed = left.Stats.Speed;
        int rightSpeed = right.Stats.Speed;

        if (left.SpecialAbility == SpecialAbilityType.Quick) leftSpeed += 10;
        if (right.SpecialAbility == SpecialAbilityType.Quick) rightSpeed += 10;

        if (leftSpeed == rightSpeed)
        {
            return UnityEngine.Random.Range(0, 2) == 0;
        }

        return leftSpeed > rightSpeed;
    }

    /// <summary>
    /// 1回の行動を処理する
    /// </summary>
    private void ProcessAction(
        MonsterData attacker,
        MonsterData defender,
        List<BattleLogEntry> logs,
        MonsterData left,
        MonsterData right,
        int turnCount)
    {
        // スタン中なら行動不能
        if (attacker.StatusAilment == StatusAilmentType.Stun)
        {
            AddLog(logs, $"{attacker.Name} は行動不能！", left, right);

            // スタンは1ターンで解除
            attacker.StatusAilment = StatusAilmentType.None;
            return;
        }

        // 行動決定
        SkillData selectedSkill = ChooseSkill(attacker, defender, turnCount);

        // スキル封印中なら通常攻撃に変える
        if (attacker.StatusAilment == StatusAilmentType.SkillSeal &&
            selectedSkill.SkillType != SkillType.NormalAttack)
        {
            AddLog(logs, $"{attacker.Name} は封印されてスキルが使えない！", left, right);

            selectedSkill = new SkillData
            {
                SkillName = "通常攻撃",
                SkillType = SkillType.NormalAttack,
                Power = 0,
                SuccessRate = 100
            };
        }

        // 行動実行
        ExecuteSkill(attacker, defender, selectedSkill, logs, left, right);
    }

    /// <summary>
    /// 性格を考慮してスキルを選ぶ
    /// </summary>
    private SkillData ChooseSkill(MonsterData attacker, MonsterData defender, int turnCount)
    {
        // スキルが無いなら通常攻撃
        if (attacker.Skills == null || attacker.Skills.Count == 0)
        {
            return new SkillData
            {
                SkillName = "通常攻撃",
                SkillType = SkillType.NormalAttack,
                Power = 0,
                SuccessRate = 100
            };
        }

        switch (attacker.Personality)
        {
            case PersonalityType.Bold:
                // 攻撃寄り
                return attacker.Skills[attacker.Skills.Count - 1];

            case PersonalityType.Calm:
                // 最初の2ターンは守り寄り
                if (turnCount <= 2)
                {
                    return attacker.Skills[0];
                }
                return attacker.Skills[attacker.Skills.Count - 1];

            case PersonalityType.Timid:
                // HPが減っていたら回復優先
                if (attacker.CurrentHp <= attacker.Stats.MaxHp / 2)
                {
                    foreach (var skill in attacker.Skills)
                    {
                        if (skill.SkillType == SkillType.Heal)
                        {
                            return skill;
                        }
                    }
                }
                return attacker.Skills[UnityEngine.Random.Range(0, attacker.Skills.Count)];

            case PersonalityType.Aggressive:
                // 低確率系を優先
                foreach (var skill in attacker.Skills)
                {
                    if (skill.SkillType == SkillType.InstantDeath || skill.SkillType == SkillType.RandomSkill)
                    {
                        return skill;
                    }
                }
                return attacker.Skills[UnityEngine.Random.Range(0, attacker.Skills.Count)];

            default:
                // その他はランダム
                return attacker.Skills[UnityEngine.Random.Range(0, attacker.Skills.Count)];
        }
    }

    /// <summary>
    /// スキルを実行する
    /// </summary>
    private void ExecuteSkill(
        MonsterData attacker,
        MonsterData defender,
        SkillData skill,
        List<BattleLogEntry> logs,
        MonsterData left,
        MonsterData right)
    {
        AddLog(logs, $"{attacker.Name} は {skill.SkillName} を使った！", left, right);

        // 成功判定
        if (UnityEngine.Random.Range(0, 100) >= skill.SuccessRate)
        {
            AddLog(logs, $"しかし失敗した！", left, right);
            return;
        }

        switch (skill.SkillType)
        {
            case SkillType.NormalAttack:
            case SkillType.StrongAttack:
                TryDamage(attacker, defender, skill.Power, attacker.Element, logs, left, right);
                break;

            case SkillType.FireMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Fire, logs, left, right);
                break;

            case SkillType.WaterMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Water, logs, left, right);
                break;

            case SkillType.GrassMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Grass, logs, left, right);
                break;

            case SkillType.ThunderMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Thunder, logs, left, right);
                break;

            case SkillType.RockMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Rock, logs, left, right);
                break;

            case SkillType.DarkMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Dark, logs, left, right);
                break;

            case SkillType.LightMagic:
                TryDamage(attacker, defender, skill.Power, ElementType.Light, logs, left, right);
                break;

            case SkillType.Heal:
                {
                    int heal = Mathf.Max(10, skill.Power);
                    int before = attacker.CurrentHp;
                    attacker.CurrentHp = Mathf.Min(attacker.Stats.MaxHp, attacker.CurrentHp + heal);
                    AddLog(logs, $"{attacker.Name} は {attacker.CurrentHp - before} 回復した！", left, right);
                    break;
                }

            case SkillType.AttackBuff:
                attacker.AttackBuff += 5;
                AddLog(logs, $"{attacker.Name} の攻撃力が上がった！", left, right);
                break;

            case SkillType.DefenseBuff:
                attacker.DefenseBuff += 5;
                AddLog(logs, $"{attacker.Name} の防御力が上がった！", left, right);
                break;

            case SkillType.CriticalBuff:
                attacker.CriticalBuff += 10;
                AddLog(logs, $"{attacker.Name} の会心率が上がった！", left, right);
                break;

            case SkillType.Invincible:
                attacker.IsInvincible = true;
                AddLog(logs, $"{attacker.Name} は1ターン無敵になった！", left, right);
                break;

            case SkillType.StunDebuff:
                defender.StatusAilment = StatusAilmentType.Stun;
                AddLog(logs, $"{defender.Name} は行動不能になった！", left, right);
                break;

            case SkillType.PoisonDebuff:
                defender.StatusAilment = StatusAilmentType.Poison;
                AddLog(logs, $"{defender.Name} は毒状態になった！", left, right);
                break;

            case SkillType.SkillSealDebuff:
                defender.StatusAilment = StatusAilmentType.SkillSeal;
                AddLog(logs, $"{defender.Name} はスキルを封印された！", left, right);
                break;

            case SkillType.DefenseDown:
                defender.DefenseBuff -= 5;
                AddLog(logs, $"{defender.Name} の防御力が下がった！", left, right);
                break;

            case SkillType.InstantDeath:
                if (UnityEngine.Random.Range(0, 100) < 15)
                {
                    defender.CurrentHp = 0;
                    AddLog(logs, $"即死が決まった！", left, right);
                }
                else
                {
                    AddLog(logs, $"即死は失敗した！", left, right);
                }
                break;

            case SkillType.RandomSkill:
                {
                    int roll = UnityEngine.Random.Range(0, 10);
                    if (roll == 0)
                    {
                        TryDamage(attacker, defender, 10, attacker.Element, logs, left, right);
                    }
                    else if (roll == 1)
                    {
                        attacker.AttackBuff += 5;
                        AddLog(logs, $"{attacker.Name} の攻撃力が上がった！", left, right);
                    }
                    else if(roll == 2) 
                    {
   
                        defender.StatusAilment = StatusAilmentType.Poison;
                        AddLog(logs, $"{defender.Name} は毒状態になった！", left, right);
                    }
                    else if(roll == 3)
                        {
                            defender.StatusAilment = StatusAilmentType.Stun;
                            AddLog(logs, $"{defender.Name} は行動不能になった！", left, right);
                        }
                    else if(roll == 4)
                    {
                        attacker.IsInvincible = true;
                        AddLog(logs, $"{attacker.Name} は1ターン無敵になった！", left, right);
                    }
                    else if(roll == 5)
                    {
                        defender.DefenseBuff -= 5;
                        AddLog(logs, $"{defender.Name} の防御力が下がった！", left, right);
                    }
                    else
                    {
                        AddLog(logs, $"しかしなにもおこらなかった！", left, right);
                    }
                    break;
                }
        }
    }

    /// <summary>
    /// 命中・回避・属性を考慮してダメージを与える
    /// </summary>
    private void TryDamage(
        MonsterData attacker,
        MonsterData defender,
        int skillPower,
        ElementType attackElement,
        List<BattleLogEntry> logs,
        MonsterData left,
        MonsterData right)
    {
        // 無敵ならダメージ無効
        if (defender.IsInvincible)
        {
            AddLog(logs, $"{defender.Name} は無敵でダメージを受けない！", left, right);
            return;
        }

        // 命中率と回避率
        int hitChance = attacker.Stats.Accuracy - (defender.Stats.Evasion + defender.Stats.Speed / 5);
        hitChance = Mathf.Clamp(hitChance, 20, 95);

        if (UnityEngine.Random.Range(0, 100) >= hitChance)
        {
            AddLog(logs, $"{defender.Name} は攻撃を回避した！", left, right);
            return;
        }

        // 実攻撃 / 実防御
        int attack = attacker.GetCurrentAttack();
        int defense = defender.GetCurrentDefense();

        // 基本ダメージ
        int damage = Mathf.Max(1, attack - defense / 2 + skillPower + UnityEngine.Random.Range(-2, 3));

        // 属性倍率
        float elementMultiplier = GetElementMultiplier(attackElement, defender.Element);
        damage = Mathf.Max(1, Mathf.RoundToInt(damage * elementMultiplier));

        // 会心
        int criticalRate = attacker.GetCurrentCriticalRate();
        if (attacker.SpecialAbility == SpecialAbilityType.Lucky)
        {
            criticalRate += 15;
        }

        bool critical = UnityEngine.Random.Range(0, 100) < criticalRate;
        if (critical)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
        }

        // 特殊能力: IronWall
        bool ironWall = defender.SpecialAbility == SpecialAbilityType.IronWall &&
                        UnityEngine.Random.Range(0, 100) < 20;
        if (ironWall)
        {
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * 0.5f));
        }

        // ダメージ反映
        defender.CurrentHp = Mathf.Max(0, defender.CurrentHp - damage);

        string extra = "";
        if (elementMultiplier > 1f) extra += " 属性有利！";
        if (elementMultiplier >= 2f) extra += " 光と闇が激突！";
        if (critical) extra += " クリティカル！";
        if (ironWall) extra += " 鉄壁で軽減！";

        AddLog(logs, $"{defender.Name} に {damage} ダメージ！{extra}", left, right);
    }

    /// <summary>
    /// ターン終了効果
    /// </summary>
    private void ApplyEndTurnEffect(
        MonsterData monster,
        List<BattleLogEntry> logs,
        MonsterData left,
        MonsterData right)
    {
        if (monster.CurrentHp <= 0) return;

        // 毒ダメージ
        if (monster.StatusAilment == StatusAilmentType.Poison)
        {
            int poisonDamage = Mathf.Max(1, monster.Stats.MaxHp / 10);
            monster.CurrentHp = Mathf.Max(0, monster.CurrentHp - poisonDamage);
            AddLog(logs, $"{monster.Name} は毒で {poisonDamage} ダメージ！", left, right);
        }

        // 成長
        bool growthTriggered = UnityEngine.Random.Range(0, 100) < monster.Stats.Growth;
        if (growthTriggered)
        {
            bool raiseAttack = UnityEngine.Random.Range(0, 2) == 0;
            if (raiseAttack)
            {
                monster.AttackBuff += UnityEngine.Random.Range(0,3);
                AddLog(logs, $"{monster.Name} は成長して攻撃力が上がった！", left, right);
            }
            else
            {
                monster.DefenseBuff += UnityEngine.Random.Range(0,3);
                AddLog(logs, $"{monster.Name} は成長して防御力が上がった！", left, right);
            }
        }

        // 特殊能力: Regenerate
        if (monster.SpecialAbility == SpecialAbilityType.Regenerate)
        {
            int heal = 3;
            int before = monster.CurrentHp;
            monster.CurrentHp = Mathf.Min(monster.Stats.MaxHp, monster.CurrentHp + heal);
            if (monster.CurrentHp > before)
            {
                AddLog(logs, $"{monster.Name} は再生した！", left, right);
            }
        }
    }

    /// <summary>
    /// 属性倍率
    /// </summary>
    private float GetElementMultiplier(ElementType attacker, ElementType defender)
    {
        // 有利属性
        if (attacker == ElementType.Water && defender == ElementType.Fire) return 1.5f;
        if (attacker == ElementType.Fire && defender == ElementType.Grass) return 1.5f;
        if (attacker == ElementType.Grass && defender == ElementType.Water) return 1.5f;
        if (attacker == ElementType.Thunder && defender == ElementType.Rock) return 1.5f;
        if (attacker == ElementType.Rock && defender == ElementType.Thunder) return 1.5f;

        // 光と闇
        if (attacker == ElementType.Dark && defender == ElementType.Light) return 2.0f;
        if (attacker == ElementType.Light && defender == ElementType.Dark) return 2.0f;

        // 被ダメ軽減側
        if (attacker == ElementType.Fire && defender == ElementType.Water) return 0.75f;
        if (attacker == ElementType.Grass && defender == ElementType.Fire) return 0.75f;
        if (attacker == ElementType.Water && defender == ElementType.Grass) return 0.75f;
        if (attacker == ElementType.Rock && defender == ElementType.Thunder) return 0.75f;
        if (attacker == ElementType.Thunder && defender == ElementType.Rock) return 0.75f;

        return 1.0f;
    }

    /// <summary>
    /// ログ追加
    /// </summary>
    private void AddLog(List<BattleLogEntry> logs, string message, MonsterData left, MonsterData right)
    {
        logs.Add(new BattleLogEntry
        {
            Message = message,
            LeftHp = left.CurrentHp,
            RightHp = right.CurrentHp
        });
    }
}