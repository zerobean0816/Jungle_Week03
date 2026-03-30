using UnityEngine;
using TMPro;

public class UIPuckPoints : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;

    void Update()
{
    if (GameManager.Instance == null) return;
    _pointsText.text = $"{GameManager.Instance.PuckPoints} pts";
}
}