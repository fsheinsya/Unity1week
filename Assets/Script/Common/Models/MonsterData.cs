using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public string Name;
    public Sprite Icon;
    public MonsterStats Stats;
    public int CurrentHp;
    public SpecialAbilityType SpecialAbility;
}