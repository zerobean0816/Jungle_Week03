using UnityEngine;
using System.Collections.Generic;
using System;

public class PuckHandler : MonoBehaviour
{
    // 슬롯 맵 대신 단순 리스트로 변경
    private List<PuckData> _equippedPucks;
    private PlayerStat _playerStat;

    public event Action OnPucksChanged;

    public int MaxListCount  = 9;
    void Start()
    {
        _playerStat = GetComponent<PlayerStat>();
        _equippedPucks = new List<PuckData>(MaxListCount);
        NullCheck.IsNull(_playerStat, "PlayerStat component not found on player.");
    }

    public List<StatModifier> GetCurrentModifiers()
    {
        List<StatModifier> allModifiers = new List<StatModifier>();

        foreach (var puck in _equippedPucks)
        {
            if (puck != null && puck.modifiers != null)
            {
                allModifiers.AddRange(puck.modifiers);
            }
        }

        return allModifiers;
    }

    /// <summary>퍽 장착</summary>
    public void EquipPuck(PuckData puckData)
    {
        if (puckData == null) return;
        if (!GameManager.Instance.CanEquipPuck(puckData) && !puckData._isNegative)
        {
            Debug.LogWarning($"Not enough points for {puckData.puckName}. Need {puckData.PointCost}, have {GameManager.Instance.PuckPoints}");
            return;
        }

        GameManager.Instance.SpendPoints(puckData);
        GameManager.Instance.PuckCounts++;
        _equippedPucks.Add(puckData);

        OnPucksChanged?.Invoke();
    }

    /// <summary>퍽 해제 — 드래그로 꺼낼 때 호출</summary>
    public void UnequipPuck(PuckData puckData)
    {
        if (puckData == null) return;

        if (puckData._isNegative)
            GameManager.Instance.SpendPoints(puckData); // ← negative pucks cost points to remove
        else
            GameManager.Instance.RefundPoints(puckData); // ← normal pucks refund on remove

        GameManager.Instance.PuckCounts--;
        _equippedPucks.Remove(puckData);

        OnPucksChanged?.Invoke();
    }

    public void EquitWithoutPoints(PuckData puckData)
    {
        GameManager.Instance.PuckCounts++;
        _equippedPucks.Add(puckData);

        OnPucksChanged?.Invoke();
    }

    public void UnequitWithoutPoints(PuckData puckData)
    {
        GameManager.Instance.PuckCounts--;
        _equippedPucks.Remove(puckData);

        OnPucksChanged?.Invoke();
    }

    public void RecalculateChangedPucks()
    {
        _playerStat.ClearModifiers();

        foreach (var puck in _equippedPucks)
            _playerStat.AddModifiers(puck.modifiers);

        _playerStat.Recalculate();
        GetComponent<PlayerController>().RecalculatePlayerState();
    }

    public void UnequitRamdon()
    {
        int _random = UnityEngine.Random.Range(0, MaxListCount - 1);
        PuckData puckData = _equippedPucks[_random];

        UnequitWithoutPoints(puckData);
    }

    public IReadOnlyList<PuckData> GetEquippedPucks() => _equippedPucks;
}
