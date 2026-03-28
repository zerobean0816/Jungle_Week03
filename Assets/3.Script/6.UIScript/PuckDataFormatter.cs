using System.Text;

public static class PuckDataFormatter
{
    public static string Format(PuckData data)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine(data.description);
        sb.AppendLine();

        foreach (var mod in data.modifiers)
            sb.AppendLine(FormatModifier(mod));

        return sb.ToString();
    }

    private static string FormatModifier(StatModifier mod)
    {
        string statName = GetStatName(mod.stat);
        string sign     = mod.value >= 0 ? "+" : "";

        if (mod.isMultiplier)
        {
            // ex) 이동속도 +10%
            float percent = mod.value * 100f;
            return $"{statName} {sign}{percent:0.#}%";
        }
        else
        {
            // ex) 최대 체력 +20
            return $"{statName} {sign}{mod.value:0.#}";
        }
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
            _                     => stat.ToString()
        };
    }
}
