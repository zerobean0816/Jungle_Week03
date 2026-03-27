using UnityEngine;

public class PuckHandler : MonoBehaviour
{
    [SerializeField] private PuckData[] activePucks = new PuckData[3]; // 최대 3개의 퍽 슬롯
    PlayerStat _playerStat;

    void Start()
    {
        _playerStat = GameManager.Instance.player.GetComponent<PlayerStat>();
        NullCheck.IsNull(_playerStat, "PlayerStat component not found on player.");
    }

    public void EquiedPuck(int slot, PuckData puckData)
    {
        activePucks[slot] = puckData;

        if (puckData == null)
        {
            RecalculateChangedPucks();
            return;
        }
        _playerStat.AddModifiers(puckData.modifiers);
    }

    public void RecalculateChangedPucks()
    {
        //Debug.Log("Clearning Modifier before recalculation...");

        _playerStat.ClearModlifiers(); // 기존 모디파이어 초기화
        if (activePucks == null || activePucks.Length == 0)
        {
            Debug.LogWarning("No active pucks found. Skipping recalculation.");
            return;
        }

        //Debug.Log("Recalculating stats based on equipped pucks...");

        foreach (var puck in activePucks)
        {
            if (puck != null)
               _playerStat.AddModifiers(puck.modifiers);
        }

        _playerStat.Recalculate(); // 최종 재계산
    }


}
