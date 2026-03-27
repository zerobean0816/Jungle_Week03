using UnityEngine;
using System.Collections.Generic;

public class PlayerStat : MonoBehaviour
{
    // ───────────────────────────────
    // Inspector에서 베이스값 설정 가능
    // ───────────────────────────────
    [Header("Base Stat")]
    public Stat stat = new Stat();

    // 활성화된 모디파이어 목록
    // ItemManager.SelectPuck() → AddModifiers() 로 채워짐
    [SerializeField] private List<StatModifier> _modifiers = new();

    // ───────────────────────────────
    // ItemManager에서 퍽 선택 시 호출
    // PuckData의 모디파이어 배열을 통째로 받음
    // ───────────────────────────────

    public void ClearModlifiers()
    {
        _modifiers.Clear();
    }

    public void Recalculate()
    {
        stat.Recalculate(_modifiers);
    }

    public void AddModifiers(StatModifier[] modifiers)
    {
        Debug.Log($"Adding {modifiers.Length} modifiers from equipped puck...");
        foreach (var mod in modifiers)
            _modifiers.Add(mod);
    }
}