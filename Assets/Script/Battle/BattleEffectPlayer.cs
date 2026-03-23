using Cysharp.Threading.Tasks;
using UnityEngine;

public class BattleEffectPlayer : MonoBehaviour
{
    [SerializeField] private BattleCharacterView leftView;
    [SerializeField] private BattleCharacterView rightView;

    public async UniTask Play(BattleLogEntry log)
    {
        if (!log.IsAction) return;
        if (!log.PlayMotion) return;

        BattleCharacterView attacker = log.IsLeftAction ? leftView : rightView;
        BattleCharacterView defender = log.IsLeftAction ? rightView : leftView;

        switch (log.UsedSkillType)
        {
            case SkillType.NormalAttack:
            case SkillType.StrongAttack:
                await attacker.PlayAttack();
                await defender.PlayHit();
                break;

            case SkillType.Heal:
                await attacker.PlayHeal();
                break;

            case SkillType.AttackBuff:
            case SkillType.DefenseBuff:
                await attacker.PlayBuff();
                break;

            case SkillType.PoisonDebuff:
                defender.ApplyPoison();
                break;
        }
    }
}