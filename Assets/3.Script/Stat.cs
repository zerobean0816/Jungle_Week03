using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    // ───────────────────────────────
    // 베이스값 (절대 직접 수정하지 말 것)
    // 퍽/상태이상 없는 순수 기본 수치
    // ───────────────────────────────
    [Header("Base Data")]
    public float baseHealth     = 100f;
    public float baseDamage     = 15f;
    public float baseMoveSpeed  = 13f;
    public float baseAttackSpeed = 2f;
    public float baseMaxStress  = 100f;

    public float baseBulletScale = 1f;
    public float basePlayerScale = 1f;

    [Range(0f , 100f)]
    public float baseAccuracy = 45f;

    public float baseHPRegen         = 1f;    // 초당 체력 회복 (기본 1)
    public float baseStressRegen     = 2f;    // 초당 스트레스 증가
    public float baseStressPerDamage = 10f;   // 피격 시 스트레스 증가량
    public float baseDamageReceived  = 1f;    // 받는 데미지 배율 (1 = 100%)

    [Header("Applying Data")]

    // ───────────────────────────────j
    // 현재값 (퍽 적용 후 실제 사용값)
    // Recalculate() 후 이 값을 읽을 것
    // ───────────────────────────────
    public float maxHealth;
    public float damage;
    public float moveSpeed;
    public float attackSpeed;
    public float maxStress;
    public float bulletScale;
    public float playerScale;
    public float accuracy;
    public float hpRegen;
    public float stressRegen;
    public float stressPerDamage;
    public float damageReceived;

    // ───────────────────────────────
    // 런타임 상태값 (실시간으로 변하는 값)
    // Recalculate() 대상 아님
    // ───────────────────────────────

    [Header("current state data")]
    public float currentHealth;
    public float currentStress;



    // 생성 시 현재값 = 베이스값으로 초기화
    public Stat()
    {
        Recalculate();
        currentHealth = maxHealth;
        currentStress = 0f;
    }

    // ───────────────────────────────
    // 퍽 장착/해제 후 반드시 호출
    // ───────────────────────────────

    public void ResetByChange()
    {
        maxHealth        = baseHealth;
        damage           = baseDamage;
        moveSpeed        = baseMoveSpeed;
        attackSpeed      = baseAttackSpeed;
        maxStress        = baseMaxStress;
        bulletScale      = baseBulletScale;
        playerScale      = basePlayerScale;
        accuracy         = baseAccuracy;
        hpRegen          = baseHPRegen;
        stressRegen      = baseStressRegen;
        stressPerDamage  = baseStressPerDamage;
        damageReceived   = baseDamageReceived;

        currentHealth = maxHealth;
    }

    public void Recalculate(List<StatModifier> modifiers = null)
    {
        float previousMaxHealth = maxHealth;

        // 1. Reset to Base
        maxHealth        = baseHealth;
        damage           = baseDamage;
        moveSpeed        = baseMoveSpeed;
        attackSpeed      = baseAttackSpeed;
        maxStress        = baseMaxStress;
        bulletScale      = baseBulletScale;
        playerScale      = basePlayerScale;
        accuracy         = baseAccuracy;
        hpRegen          = baseHPRegen;
        stressRegen      = baseStressRegen;
        stressPerDamage  = baseStressPerDamage;
        damageReceived   = baseDamageReceived;

        if (modifiers == null || modifiers.Count == 0) 
        {
            FinalizeStats(previousMaxHealth);
            return;
        }

        // 2. Aggregate Multipliers
        Dictionary<StatType, float> multiplierSums = new Dictionary<StatType, float>();
        foreach (StatType type in Enum.GetValues(typeof(StatType)))
            multiplierSums[type] = 0f;

        foreach (var mod in modifiers)
        {
            if (mod.isMultiplier)
                multiplierSums[mod.stat] += mod.value;
            else
                ApplyFlatModifier(mod);
        }

        // 3. Apply Multipliers with a Floor
        // We use a helper function to prevent the (1 + sum) from going below 0 or a specific minimum.
        maxHealth        = ApplyModWithFloor(maxHealth,        multiplierSums[StatType.MaxHP]);
        damage           = ApplyModWithFloor(damage,           multiplierSums[StatType.AttackPower]);
        moveSpeed        = ApplyModWithFloor(moveSpeed,        multiplierSums[StatType.MoveSpeed], 0.1f); // Minimum 10% speed
        attackSpeed      = ApplyModWithFloor(attackSpeed,      multiplierSums[StatType.AttackSpeed], 0.1f);
        maxStress        = ApplyModWithFloor(maxStress,        multiplierSums[StatType.MaxStress]);
        bulletScale      = ApplyModWithFloor(bulletScale,      multiplierSums[StatType.BulletScale], 0.1f);
        playerScale      = ApplyModWithFloor(playerScale,      multiplierSums[StatType.PlayerScale], 0.1f);
        accuracy         = ApplyModWithFloor(accuracy,         multiplierSums[StatType.Accuracy]);
        hpRegen          = ApplyModWithFloor(hpRegen,          multiplierSums[StatType.HPRegen], -100f); // Regen can be negative (poison)?
        stressRegen      = ApplyModWithFloor(stressRegen,      multiplierSums[StatType.StressRegen]);
        stressPerDamage  = ApplyModWithFloor(stressPerDamage,  multiplierSums[StatType.StressPerDamage]);
        damageReceived   = ApplyModWithFloor(damageReceived,   multiplierSums[StatType.DamageReceived], 0f);

        FinalizeStats(previousMaxHealth);
    }

    // Helper to handle the math safely
    private float ApplyModWithFloor(float baseVal, float multiplierSum, float absoluteMin = 0f)
    {
        // Ensure the multiplier itself doesn't result in a value less than 0
        // e.g., if multiplierSum is -1.5, (1 + -1.5) = -0.5. We clamp that.
        float finalMultiplier = Mathf.Max(0f, 1f + multiplierSum);
        
        // Calculate final value and ensure it doesn't cross the absolute minimum floor
        return Mathf.Max(absoluteMin, baseVal * finalMultiplier);
    }

    private void FinalizeStats(float previousMaxHealth)
    {
        ApplyHealthRatio(previousMaxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    // 덧셈 전용 함수
    private void ApplyFlatModifier(StatModifier mod)
    {
        switch (mod.stat)
    {
        case StatType.MaxHP:          maxHealth       += mod.value; break;
        case StatType.AttackPower:    damage          += mod.value; break;
        case StatType.MoveSpeed:      moveSpeed       += mod.value; break;
        case StatType.AttackSpeed:    attackSpeed     += mod.value; break;
        case StatType.MaxStress:      maxStress       += mod.value; break;
        case StatType.BulletScale:    bulletScale     += mod.value; break;
        case StatType.PlayerScale:    playerScale     += mod.value; break;
        case StatType.Accuracy:       accuracy        += mod.value; break;
        case StatType.HPRegen:        hpRegen         += mod.value; break;
        case StatType.StressRegen:    stressRegen     += mod.value; break;
        case StatType.StressPerDamage:stressPerDamage += mod.value; break;
        case StatType.DamageReceived: damageReceived  += mod.value; break;
    }
    }

    public void ApplyHealthRatio(float previousMaxHealth)
    {
        float healthRatio = previousMaxHealth > 0f ? currentHealth / previousMaxHealth : 1f;
        currentHealth = Mathf.Clamp(maxHealth * healthRatio, 0f, maxHealth);
    }

    public Stat GetPreview(List<StatModifier> modifiers)
    {
        Stat preview = new Stat();

        // 베이스값 복사
        preview.baseHealth      = baseHealth;
        preview.baseDamage      = baseDamage;
        preview.baseMoveSpeed   = baseMoveSpeed;
        preview.baseAttackSpeed = baseAttackSpeed;
        preview.baseMaxStress   = baseMaxStress;
        preview.baseBulletScale = baseBulletScale;
        preview.basePlayerScale = basePlayerScale;
        preview.baseAccuracy    = baseAccuracy;

        // ✅ 현재 런타임 상태도 복사 (미리보기용 HP 비율 계산에 필요)
        preview.currentHealth = currentHealth;
        preview.currentStress = currentStress;
        preview.maxHealth     = maxHealth; // 비율 계산 기준점

        // 원본 stat은 절대 건드리지 않음
        preview.Recalculate(modifiers);
        return preview; // 복사본만 반환
    }
}
