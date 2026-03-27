using System;

public enum StatType
{
    MaxHP,
    AttackPower,
    MoveSpeed,
    AttackSpeed,
    MaxStress
}

public enum ModifierType
{
    Additive,       // 플랫 덧셈  ex) +10
    Multiplicative  // 배율 곱셈  ex) +10%
}

[Serializable]
public class StatModifier
{
    public StatType stat;       // 어떤 스탯에 적용되는지
    public float value;         // 플랫값 또는 배율값 (ex: 0.1f = +10%)
    public bool isMultiplier;   // true면 배율, false면 플랫

    public StatModifier(StatType stat, float value, bool isMultiplier)
    {
        this.stat = stat;
        this.value = value;
        this.isMultiplier = isMultiplier;
    }
}
