using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// モンスター生成クラス
/// UIに出したいパラメータをここで全部埋める
/// </summary>
public class MonsterFactory
{
    /// <summary>
    /// ランダムなモンスターを1体作る
    /// </summary>
    public MonsterData CreateRandom(string monsterName, Sprite icon = null)
    {
        // 基本能力値をランダム生成
        int hp = Random.Range(45, 99);
        int atk = Random.Range(8, 26);
        int def = Random.Range(8, 26);
        int growth = Random.Range(5, 31);
        int speed = Random.Range(5, 31);
        int accuracy = Random.Range(65, 96);
        int evasion = Random.Range(2, 10);
        int critical = Random.Range(5, 14);

        // 属性、性格、特殊能力をランダムで決める
        ElementType element = GetRandomElement();
        PersonalityType personality = GetRandomPersonality();
        SpecialAbilityType specialAbility = GetRandomSpecialAbility();

        // スキルを2つ作る
        List<SkillData> skills = new List<SkillData>
        {
            CreateSkillByPersonality(personality, element, 0),
            CreateSkillByPersonality(personality, element, 1)
        };

        // モンスターを返す
        return new MonsterData
        {
            Name = monsterName,
            Icon = icon,
            Stats = new MonsterStats
            {
                MaxHp = hp,
                Attack = atk,
                Defense = def,
                Growth = growth,
                Speed = speed,
                Accuracy = accuracy,
                Evasion = evasion,
                CriticalRate = critical
            },
            CurrentHp = hp,
            SpecialAbility = specialAbility,
            Element = element,
            Personality = personality,
            StatusAilment = StatusAilmentType.None,
            Skills = skills,
            AttackBuff = 0,
            DefenseBuff = 0,
            CriticalBuff = 0,
            IsInvincible = false,
            IsStunnedThisTurn = false
        };
    }

    /// <summary>
    /// ランダム属性
    /// </summary>
    private ElementType GetRandomElement()
    {
        int value = Random.Range(0, 8);
        return (ElementType)value;
    }

    /// <summary>
    /// ランダム性格
    /// </summary>
    private PersonalityType GetRandomPersonality()
    {
        int value = Random.Range(0, 7);
        return (PersonalityType)value;
    }

    /// <summary>
    /// ランダム特殊能力
    /// </summary>
    private SpecialAbilityType GetRandomSpecialAbility()
    {
        int value = Random.Range(0, 7);
        return (SpecialAbilityType)value;
    }

    /// <summary>
    /// 性格に応じてスキルを作る
    /// </summary>
    private SkillData CreateSkillByPersonality(PersonalityType personality, ElementType element, int index)
    {
        // 基本スキル
        SkillData normalAttack = new SkillData
        {
            SkillName = "通常攻撃",
            SkillType = SkillType.NormalAttack,
            Power = 0,
            SuccessRate = 100
        };

        // 属性魔法
        SkillData elementSkill = CreateElementSkill(element);

        // 性格ごとにスキル傾向を変える
        switch (personality)
        {
            case PersonalityType.Bold:
                return index == 0 ? normalAttack : new SkillData
                {
                    SkillName = "強撃",
                    SkillType = SkillType.StrongAttack,
                    Power = 12,
                    SuccessRate = 85
                };

            case PersonalityType.Calm:
                return index == 0 ? new SkillData
                {
                    SkillName = "防御集中",
                    SkillType = SkillType.DefenseBuff,
                    Power = 0,
                    SuccessRate = 100
                } : elementSkill;

            case PersonalityType.Flexible:
                return index == 0 ? normalAttack : new SkillData
                {
                    SkillName = "まねる",
                    SkillType = SkillType.RandomSkill,
                    Power = 0,
                    SuccessRate = 100
                };

            case PersonalityType.Taunt:
                return index == 0 ? new SkillData
                {
                    SkillName = "挑発の構え",
                    SkillType = SkillType.AttackBuff,
                    Power = 0,
                    SuccessRate = 100
                } : new SkillData
                {
                    SkillName = "反撃強打",
                    SkillType = SkillType.StrongAttack,
                    Power = 15,
                    SuccessRate = 75
                };

            case PersonalityType.Aggressive:
                return index == 0 ? new SkillData
                {
                    SkillName = "即死狙い",
                    SkillType = SkillType.InstantDeath,
                    Power = 0,
                    SuccessRate = 15
                } : new SkillData
                {
                    SkillName = "乱数魔技",
                    SkillType = SkillType.RandomSkill,
                    Power = 0,
                    SuccessRate = 100
                };

            case PersonalityType.Timid:
                return index == 0 ? new SkillData
                {
                    SkillName = "回復",
                    SkillType = SkillType.Heal,
                    Power = 20,
                    SuccessRate = 100
                } : new SkillData
                {
                    SkillName = "毒霧",
                    SkillType = SkillType.PoisonDebuff,
                    Power = 0,
                    SuccessRate = 70
                };

            default:
                return index == 0 ? normalAttack : elementSkill;
        }
    }

    /// <summary>
    /// 属性に対応した魔法スキルを返す
    /// </summary>
    private SkillData CreateElementSkill(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                return new SkillData { SkillName = "炎魔法", SkillType = SkillType.FireMagic, Power = 10, SuccessRate = 90 };
            case ElementType.Water:
                return new SkillData { SkillName = "水魔法", SkillType = SkillType.WaterMagic, Power = 10, SuccessRate = 90 };
            case ElementType.Grass:
                return new SkillData { SkillName = "草魔法", SkillType = SkillType.GrassMagic, Power = 10, SuccessRate = 90 };
            case ElementType.Thunder:
                return new SkillData { SkillName = "雷魔法", SkillType = SkillType.ThunderMagic, Power = 10, SuccessRate = 90 };
            case ElementType.Rock:
                return new SkillData { SkillName = "岩魔法", SkillType = SkillType.RockMagic, Power = 10, SuccessRate = 90 };
            case ElementType.Dark:
                return new SkillData { SkillName = "闇魔法", SkillType = SkillType.DarkMagic, Power = 12, SuccessRate = 85 };
            case ElementType.Light:
                return new SkillData { SkillName = "光魔法", SkillType = SkillType.LightMagic, Power = 12, SuccessRate = 85 };
            default:
                return new SkillData { SkillName = "通常攻撃", SkillType = SkillType.NormalAttack, Power = 0, SuccessRate = 100 };
        }
    }
}