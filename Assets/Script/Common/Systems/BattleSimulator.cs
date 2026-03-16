using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator
{
    public MatchResultData Simulate(MonsterData left, MonsterData right, BetData betData)
    {
        var logs = new List<string>();

        left.CurrentHp = left.Stats.MaxHp;
        right.CurrentHp = right.Stats.MaxHp;

        logs.Add($"{left.Name} と {right.Name} の戦闘開始！");

        while (left.CurrentHp > 0 && right.CurrentHp > 0)
        {
            bool isLeftFirst = ShouldLeftActFirst(left, right);

            if (isLeftFirst)
            {
                Attack(left, right, logs);
                if (right.CurrentHp <= 0) break;

                Attack(right, left, logs);
            }
            else
            {
                Attack(right, left, logs);
                if (left.CurrentHp <= 0) break;

                Attack(left, right, logs);
            }

            ApplyTurnEndEffects(left, logs);
            if (left.CurrentHp <= 0) break;

            ApplyTurnEndEffects(right, logs);
        }

        PredictionSide winnerSide = left.CurrentHp > 0 ? PredictionSide.Left : PredictionSide.Right;
        string winnerName = left.CurrentHp > 0 ? left.Name : right.Name;

        bool isPredictionSuccess = betData != null && betData.Side == winnerSide;
        int reward = isPredictionSuccess ? betData.Amount * 2 : 0;

        logs.Add($"{winnerName} の勝利！");

        return new MatchResultData
        {
            WinnerName = winnerName,
            WinnerSide = winnerSide,
            IsPredictionSuccess = isPredictionSuccess,
            RewardCoin = reward,
            BattleLogs = logs
        };
    }

    private bool ShouldLeftActFirst(MonsterData left, MonsterData right)
    {
        bool leftQuick = left.SpecialAbility == SpecialAbilityType.Quick;
        bool rightQuick = right.SpecialAbility == SpecialAbilityType.Quick;

        if (leftQuick && !rightQuick) return true;
        if (!leftQuick && rightQuick) return false;

        return Random.Range(0, 2) == 0;
    }

    private void Attack(MonsterData attacker, MonsterData defender, List<string> logs)
    {
        int attackPower = attacker.Stats.Attack;

        if (attacker.SpecialAbility == SpecialAbilityType.Berserk &&
            attacker.CurrentHp <= attacker.Stats.MaxHp / 2)
        {
            attackPower += 5;
            logs.Add($"{attacker.Name} は狂化した！ ATKが上がった！");
        }

        int critRate = 10;
        if (attacker.SpecialAbility == SpecialAbilityType.Lucky)
        {
            critRate += 15;
        }

        int damage = Mathf.Max(1, attackPower - defender.Stats.Defense / 2 + Random.Range(-2, 3));

        bool powerStrike = attacker.SpecialAbility == SpecialAbilityType.PowerStrike &&
                           Random.Range(0, 100) < 20;
        if (powerStrike)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
        }

        bool critical = Random.Range(0, 100) < critRate;
        if (critical)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
        }

        bool ironWall = defender.SpecialAbility == SpecialAbilityType.IronWall &&
                        Random.Range(0, 100) < 20;
        if (ironWall)
        {
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * 0.5f));
        }

        defender.CurrentHp = Mathf.Max(0, defender.CurrentHp - damage);

        string extraText = "";
        if (powerStrike) extraText += " 強打！";
        if (critical) extraText += " クリティカル！";
        if (ironWall) extraText += " 鉄壁で軽減！";

        logs.Add($"{attacker.Name} の攻撃！ {defender.Name} に {damage} ダメージ！{extraText}");
        logs.Add($"{defender.Name} の残りHP: {defender.CurrentHp}");
    }

    private void ApplyTurnEndEffects(MonsterData monster, List<string> logs)
    {
        if (monster.CurrentHp <= 0) return;

        if (monster.SpecialAbility == SpecialAbilityType.Regenerate)
        {
            int heal = 3;
            int beforeHp = monster.CurrentHp;
            monster.CurrentHp = Mathf.Min(monster.Stats.MaxHp, monster.CurrentHp + heal);

            if (monster.CurrentHp > beforeHp)
            {
                logs.Add($"{monster.Name} は再生した！ HPが {monster.CurrentHp - beforeHp} 回復！");
            }
        }

        bool growthTriggered = Random.Range(0, 100) < monster.Stats.Growth;
        if (growthTriggered)
        {
            bool raiseAttack = Random.Range(0, 2) == 0;

            if (raiseAttack)
            {
                monster.Stats.Attack += 1;
                logs.Add($"{monster.Name} は成長した！ ATKが1上がった！");
            }
            else
            {
                monster.Stats.Defense += 1;
                logs.Add($"{monster.Name} は成長した！ DEFが1上がった！");
            }
        }
    }
}
