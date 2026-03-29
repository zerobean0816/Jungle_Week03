using System.Text;

public static class StatDataFormator
{
    public static string Format(Stat data)
    {
        StringBuilder sb = new StringBuilder();

        // --- Base Stats ---
        sb.AppendLine($"{"최대 체력",-14}: {data.maxHealth}");
        sb.AppendLine($"{"최대 스트레스",-12}: {data.maxStress}");
        sb.AppendLine($"{"정확도",-16}: {data.accuracy}");
        sb.AppendLine($"{"공격력",-16}: {data.damage}"); 
        
        sb.AppendLine(); // Adds a blank line space!

        // --- Speed Stats ---
        sb.AppendLine($"{"공격 속도",-14}: {data.attackSpeed}");
        sb.AppendLine($"{"이동 속도",-14}: {data.moveSpeed}");
        
        sb.AppendLine(); // Adds another blank line space!

        // --- Scale Stats ---
        sb.AppendLine($"{"플레이어 크기",-12}: {data.playerScale}"); 
        sb.AppendLine($"{"총알 크기",-14}: {data.bulletScale}");

        sb.AppendLine(); // Adds another blank line space!

        // --- Additional ---
        sb.AppendLine($"{"초당 체력 회복",-12}: {data.hpRegen}"); 
        sb.AppendLine($"{"초당 스트레스",-13}: {data.stressRegen}");

        sb.AppendLine(); // Adds another blank line space!

        sb.AppendLine($"{"피격당 스트레스",-11}: {data.stressPerDamage}");
        sb.AppendLine($"{"피격당 추가피해",-11}: {data.damageReceived}");


        return sb.ToString();
    }
}
