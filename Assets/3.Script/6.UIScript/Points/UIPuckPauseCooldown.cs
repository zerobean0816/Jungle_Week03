using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPuckPauseCooldown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cooldownText;
    [SerializeField] private Button _openButton;

    void Update()
    {
        if (GameManager.Instance == null) return;

        bool ready = GameManager.Instance.CanOpenPuckPause;
        _openButton.interactable = ready;
        _cooldownText.text = ready 
            ? "Ready" 
            : GameManager.Instance.PuckPauseCooldownRemaining.ToString("F1") + "s";
    }
}