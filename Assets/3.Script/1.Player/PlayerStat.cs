using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerStat : MonoBehaviour
{
    public event Action OnStatChanged;
    public event Action<Stat> OnPreviewStat; // UI에 미리보기 전달

    [Header("Base Stat")]
    public Stat stat = new Stat();
    public List<StatModifier> Modifiers { get; private set; }

    private List<StatModifier> _pendingModifiers = new List<StatModifier>(); // 대기 중인 모디파이어
    private float _savedHealthRatio = 1f;

    void Start()
    {
        Modifiers = new List<StatModifier>();
    }

    // 퍽 선택 시 → 미리보기만, 실제 적용 X
    public void PreviewModifiers(StatModifier[] modifiers)
    {
        _pendingModifiers = new List<StatModifier>(Modifiers); // 현재 모디파이어 복사
        foreach (var mod in modifiers)
            _pendingModifiers.Add(mod);

        Stat preview = stat.GetPreview(_pendingModifiers);
        OnPreviewStat?.Invoke(preview); // UI에 미리보기 전달
    }

    // 퍽 확정 시 → 실제 적용
    public void ConfirmModifiers()
    {
        Modifiers = new List<StatModifier>(_pendingModifiers);
        _pendingModifiers.Clear();
        Recalculate();

        stat.currentHealth = Mathf.Clamp(stat.maxHealth * _savedHealthRatio, 0f, stat.maxHealth);
    }

    // 퍽 취소 시 → 대기 목록 초기화
    public void CancelPreview()
    {
        _pendingModifiers.Clear();

        stat.currentHealth = Mathf.Clamp(stat.maxHealth * _savedHealthRatio, 0f, stat.maxHealth);
    }

    public void ClearModifiers()
    {
        Modifiers.Clear();
    }

    public void Recalculate()
    {
        stat.Recalculate(Modifiers);
        GetComponent<PlayerController>().RecalculatePlayerState();
        OnStatChanged?.Invoke();
    }

    public void AddModifiers(StatModifier[] modifiers)
    {
        foreach (var mod in modifiers)
            Modifiers.Add(mod);
    }

    public void EnterPuckPause()
    {
        // 현재 HP 비율 스냅샷 저장
        _savedHealthRatio = stat.maxHealth > 0f 
            ? stat.currentHealth / stat.maxHealth 
            : 1f;

        Stat preview = stat.GetPreview(Modifiers);
        OnPreviewStat?.Invoke(preview);
    }
}