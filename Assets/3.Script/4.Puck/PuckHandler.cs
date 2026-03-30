using UnityEngine;
using System.Collections.Generic;

public class PuckHandler : MonoBehaviour
{
    // 슬롯 맵 대신 단순 리스트로 변경
    private List<PuckData> _equippedPucks;
    private PlayerStat _playerStat;

    public int MaxListCount  = 12;
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

        //if (_equippedPucks.Contains(puckData)) return;

        GameManager.Instance.PuckCounts ++;
        //Debug.Log("Packed! " + GameManager.Instance.PuckCounts);

        _equippedPucks.Add(puckData);
    }

    /// <summary>퍽 해제 — 드래그로 꺼낼 때 호출</summary>
    public void UnequipPuck(PuckData puckData)
    {
        if (puckData == null) return;

        GameManager.Instance.PuckCounts --;
       // Debug.Log("UnPacked!" + GameManager.Instance.PuckCounts);

        _equippedPucks.Remove(puckData);
    }

    public void RecalculateChangedPucks()
    {
        _playerStat.ClearModifiers();

        foreach (var puck in _equippedPucks)
            _playerStat.AddModifiers(puck.modifiers);

        _playerStat.Recalculate();
        GetComponent<PlayerController>().RecalculatePlayerState();
    }

    public IReadOnlyList<PuckData> GetEquippedPucks() => _equippedPucks;
}
