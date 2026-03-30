
using UnityEngine;

public class PlayerStressControl : MonoBehaviour
{
    [SerializeField] private UIPuckSlotContainer _slotContainer; // assign in Inspector
    [SerializeField] private GameObject _puckItemPrefab;

    private PlayerStat _playerStat;
    private PuckHandler _puchHandler;
    private bool _isBroken = false; // Add this boolean flag!
    private bool has_Warned = false;

    void Start()
    {
        _playerStat = GetComponent<PlayerStat>();
        _puchHandler = GetComponent<PuckHandler>();
    }

    public void AddStress(float amount, Vector3 _position = default)
    {
        if (_isBroken) return;

        _playerStat.stat.currentStress += amount;
        _playerStat.stat.currentStress = Mathf.Clamp(_playerStat.stat.currentStress, 0f, _playerStat.stat.maxStress); // ← clamp so it doesn't go below 0

        float currentStressRate = _playerStat.stat.currentStress / _playerStat.stat.maxStress;
        if (currentStressRate > 0.7 && !has_Warned)
        {
            FloatingTextSpawner.Instance.PoolText( "스트레스 수치가 높습니다..", Color.red, transform.position, 1.5f);
            has_Warned = true;
        }

        // Only trigger breakdown on positive stress overflow
        if (amount > 0 && _playerStat.stat.currentStress >= _playerStat.stat.maxStress)
        {
            TriggerBreakdown(_position);
            _playerStat.stat.currentStress = 0;
        }
    }

    private void TriggerBreakdown(Vector2 position)
    {
        if (GameManager.Instance.StressPuckData == null ||
        GameManager.Instance.StressPuckData.Count == 0) return;

        int index = Random.Range(0, GameManager.Instance.StressPuckData.Count);
        PuckData debuff = GameManager.Instance.StressPuckData[index];

        // Data only
        GameManager.Instance.EarnPoints(2);

        if (_puchHandler.MaxListCount == GameManager.Instance.PuckCounts)
        {
            _puchHandler.UnequitRamdon();
        }
        _puchHandler.EquitWithoutPoints(debuff);
        _puchHandler.RecalculateChangedPucks();

        // Refresh UI directly
        _slotContainer?.RefreshUI();
        has_Warned = false;

        FloatingTextSpawner.Instance.PoolText(debuff.puckName, Color.red, position, 2f);
    }

    // Call this whenever the player recovers or returns to normal state
    public void ResetBreakdown()
    {
        _isBroken = false;
    }
}
