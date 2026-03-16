using UnityEngine;
public class MonsterFactory
{
    public MonsterData CreateRandom(string monsterName, Sprite icon = null)
    {
        //能力値をランダムに設定
        int hp = Random.Range(45, 81);
        int atk = Random.Range(10, 26);
        int def = Random.Range(8, 21);
        int growth = Random.Range(5, 31);

        return new MonsterData
        {
            Name = monsterName,
            Icon = icon,
            Stats = new MonsterStats
            {
                MaxHp = hp,
                Attack = atk,
                Defense = def,
                Growth = growth
            },
            CurrentHp = hp,
            SpecialAbility = GetRandomAbility()
        };
    }

    private SpecialAbilityType GetRandomAbility()
    {
        int value = Random.Range(0, 7);
        return (SpecialAbilityType)value;
    }
}
