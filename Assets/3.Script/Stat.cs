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
    public float baseHealth     = 100f;
    public float baseDamage     = 10f;
    public float baseMoveSpeed  = 7f;
    public float baseAttackSpeed = 1f;
    public float baseMaxStress  = 100f;

    public float baseBulletScale = 1f;
    public float basePlayerSclae = 1f;
    public float baseAccuracy = 25f;

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
    public float playerSclae;
    public float accuracy;

    // ───────────────────────────────
    // 런타임 상태값 (실시간으로 변하는 값)
    // Recalculate() 대상 아님
    // ───────────────────────────────
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
    public void Recalculate(List<StatModifier> modifiers = null)
    {
        maxHealth    = baseHealth;
        damage       = baseDamage;
        moveSpeed    = baseMoveSpeed;
        attackSpeed  = baseAttackSpeed;
        maxStress    = baseMaxStress;
        bulletScale  = baseBulletScale;
        playerSclae  = basePlayerSclae;
        accuracy     = baseAccuracy;
        
        //Debug.Log("[Stat] : Recalculating stats...");

        if (modifiers == null || modifiers.Count == 0) 
        {
            //Debug.Log("[Stat] : No modifiers to apply.");
            return;
        }

        // 2. 플랫(덧셈) 먼저
        foreach (var mod in modifiers)
            if (!mod.isMultiplier) ApplyModifier(mod);


        // 3. 배율(곱셈) 나중에
        foreach (var mod in modifiers)
            if (mod.isMultiplier) ApplyModifier(mod);

        //Debug.Log($"[Stat] : Recalculation complete. MaxHealth={maxHealth}, Damage={damage}, MoveSpeed={moveSpeed}, AttackSpeed={attackSpeed}, MaxStress={maxStress}");

        // currentHealth는 maxHealth 초과 불가
        currentHealth = Mathf.Min(currentHealth, maxHealth);
    }

    private void ApplyModifier(StatModifier mod)
    {
        switch (mod.stat)
        {
            case StatType.MaxHP:
                maxHealth    = mod.isMultiplier ? maxHealth      * (1f + mod.value) : maxHealth    + mod.value;
                break;
            case StatType.AttackPower:
                damage       = mod.isMultiplier ? damage         * (1f + mod.value) : damage       + mod.value;
                break;
            case StatType.MoveSpeed:
                moveSpeed    = mod.isMultiplier ? moveSpeed      * (1f + mod.value) : moveSpeed    + mod.value;
                break;
            case StatType.AttackSpeed:
                attackSpeed  = mod.isMultiplier ? attackSpeed    * (1f + mod.value) : attackSpeed  + mod.value;
                break;
            case StatType.MaxStress:
                maxStress    = mod.isMultiplier ? maxStress      * (1f + mod.value) : maxStress    + mod.value;
                break;
            case StatType.BulletScale:
                bulletScale    = mod.isMultiplier ? bulletScale  * (1f + mod.value) : bulletScale    + mod.value;
                break;
            case StatType.PlayerScale:
                playerSclae    = mod.isMultiplier ? playerSclae  * (1f + mod.value) : playerSclae    + mod.value;
                break;
            case StatType.Accuracy:
                maxStress    = mod.isMultiplier ? accuracy       * (1f + mod.value) : accuracy    + mod.value;
                break;
        }
    }
}
