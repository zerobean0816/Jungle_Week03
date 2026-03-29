using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _statShowText;
    
    private PlayerStat _playerStat;

    void Start()
    {
        _playerStat = GameManager.Instance.Player.GetComponent<PlayerStat>();
        _playerStat.OnStatChanged  += ShowPlayerStats;         // 실제 적용 시
        _playerStat.OnPreviewStat  += ShowPreviewStats;        // 미리보기 시

        ShowPlayerStats();
    }

    void OnEnable()
    {
        if (_playerStat == null) return;

        // 패널이 열릴 때 PuckPause 중이면 미리보기, 아니면 실제 스탯 표시
        if (GameManager.Instance.GameState == GameState.PuckPaused)
        {
            Stat preview = _playerStat.stat.GetPreview(_playerStat.Modifiers);
            ShowPreviewStats(preview);
        }
        else
        {
            ShowPlayerStats();
        }
    }

    void OnDestroy()
    {
        _playerStat.OnStatChanged  -= ShowPlayerStats;
        _playerStat.OnPreviewStat  -= ShowPreviewStats;
    }

    // 미리보기 — 임시 Stat을 받아서 표시
    private void ShowPreviewStats(Stat previewStat)
    {
        _nameText.text = "Player Stats (Preview)";
        _statShowText.text = StatDataFormator.Format(previewStat);
    }

    public void ShowPlayerStats()
    {
        _nameText.text = "Player Stats";
        _statShowText.text = StatDataFormator.Format(_playerStat.stat);
    }
}
