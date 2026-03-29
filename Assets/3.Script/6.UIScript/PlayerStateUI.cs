using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    private GameObject _player;
    private PlayerStat _playerStat;
    private PlayerAction _playerAction;

    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _stressSlider;
    [SerializeField] private Slider _reloadSlider;

    RectTransform _hpRect;
    RectTransform _stressRect;
    RectTransform _reloadRect;

    private float _baseWidth = 2f;

    void Start()
    {
        _player = GameManager.Instance.Player;
        _playerStat = _player.GetComponent<PlayerStat>();
        _playerAction = _player.GetComponent<PlayerAction>();

        _playerStat.OnStatChanged += ChangeSliderLength;

        _hpRect = _hpSlider.GetComponent<RectTransform>();
        _stressRect = _stressSlider.GetComponent<RectTransform>();
        _reloadRect = _reloadSlider.GetComponent<RectTransform>();

        ChangeSliderLength();
    }

    void Update()
    {
        _hpSlider.value     = _playerStat.stat.currentHealth / _playerStat.stat.maxHealth;
        _stressSlider.value = _playerStat.stat.currentStress / _playerStat.stat.maxStress;
        _reloadSlider.value = _playerAction.GetReloadProgress(); // 0 ~ 1
    }


    void ChangeSliderLength()
    {
        float hpScale     = _playerStat.stat.maxHealth  / _playerStat.stat.baseHealth;
        float stressScale = _playerStat.stat.maxStress  / _playerStat.stat.baseMaxStress;
        float reloadScale = _playerStat.stat.attackSpeed / _playerStat.stat.baseAttackSpeed; 

        _hpRect.sizeDelta     = new Vector2(_baseWidth * hpScale,     _hpRect.sizeDelta.y);
        _stressRect.sizeDelta = new Vector2(_baseWidth * stressScale, _stressRect.sizeDelta.y);
        _reloadRect.sizeDelta = new Vector2(_baseWidth / reloadScale, _reloadRect.sizeDelta.y);
    }
}
