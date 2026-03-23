using UnityEngine;

public class DamageCalculator
{
    public int CalculateDamage(MonsterData attacker, MonsterData defender, int power, ElementType element)
    {
        int attack = attacker.GetCurrentAttack();
        int defense = defender.GetCurrentDefense();

        int damage = Mathf.Max(1, attack - defense / 2 + power + Random.Range(-2, 3));

        float multiplier = GetElementMultiplier(element, defender.Element);
        damage = Mathf.RoundToInt(damage * multiplier);

        return Mathf.Max(1, damage);
    }

    private float GetElementMultiplier(ElementType attacker, ElementType defender)
    {
        if (attacker == ElementType.Water && defender == ElementType.Fire) return 1.5f;
        if (attacker == ElementType.Fire && defender == ElementType.Grass) return 1.5f;
        if (attacker == ElementType.Grass && defender == ElementType.Water) return 1.5f;

        if (attacker == ElementType.Dark && defender == ElementType.Light) return 2f;
        if (attacker == ElementType.Light && defender == ElementType.Dark) return 2f;

        return 1f;
    }
}