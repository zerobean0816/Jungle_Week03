using System.Text;

public static class PuckDataFormatter
{
    public static string Format(PuckData data)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var mod in data.modifiers)
            sb.AppendLine(FormatModifier(mod));

        return sb.ToString();
    }

    private static string FormatModifier(StatModifier mod)
    {
        string statName = GetStatName(mod.stat);
        string sign     = mod.value >= 0 ? "+" : "";

        string value;
        if (mod.isMultiplier)
        {
            float percent = mod.value * 100f;
            value = $"{sign}{percent:0.#}%";
        }
        else
        {
            value = $"{sign}{mod.value:0.#}";
        }

        string color = mod.value >= 0 ? "#4FC3F7" : "#EF9A9A"; // 파란색 / 빨간색
        return $"{statName} <color={color}>{value}</color>";
    }

    private static string GetStatName(StatType stat)
    {
        return stat switch
        {
            StatType.MaxHP        => "최대 체력",
            StatType.AttackPower  => "공격력",
            StatType.MoveSpeed    => "이동속도",
            StatType.AttackSpeed  => "공격속도",
            StatType.MaxStress    => "최대 스트레스",
            StatType.BulletScale  => "탄환 크기",
            StatType.PlayerScale  => "플레이어 크기",
            StatType.Accuracy     => "정확도",
            StatType.HPRegen         => "체력 재생",
            StatType.StressRegen     => "스트레스 증가",
            StatType.StressPerDamage => "피격 스트레스",
            StatType.DamageReceived  => "받는 데미지",
            _                     => stat.ToString()
        };
    }
}
