
using UnityEngine;

public class PlayerStressControl : MonoBehaviour
{

    private PlayerStat _playerStat;
    private PuckHandler _puchHandler;
    private bool _isBroken = false; // Add this boolean flag!

    void Start()
    {
        _playerStat = GetComponent<PlayerStat>();
        _puchHandler = GetComponent<PuckHandler>();
    }

    public void AddStress(float amount, Vector3 _position = default)
    {
        // 1. If we are already broken, immediately stop doing math or triggering texts!
        if (_isBroken) return; 

        _playerStat.stat.currentStress += amount;

        if (_playerStat.stat.currentStress >= _playerStat.stat.maxStress)
        {
            TriggerBreakdown(_position);
            _playerStat.stat.currentStress = 0;

        }
    }

    private void TriggerBreakdown(Vector2 position)
    {
        int _debuffValue = Random.Range(0, GameManager.Instance.StressPuckData.Count);
        PuckData debuff = GameManager.Instance.StressPuckData[_debuffValue];

        _puchHandler.EquipPuck(debuff);
        _puchHandler.RecalculateChangedPucks();

        FloatingTextSpawner.Instance.PoolText(debuff.puckName, Color.red,  position);
    }

    // Call this whenever the player recovers or returns to normal state
    public void ResetBreakdown()
    {
        _isBroken = false;
    }
}
