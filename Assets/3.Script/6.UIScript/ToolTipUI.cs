using UnityEngine;

using TMPro;
using UnityEngine.InputSystem;

public class ToolTipUI : MonoBehaviour
{
    public static ToolTipUI Instance;

    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _statShowText;
    
    private Canvas _parentCanvas; // Drag your Main Canvas here
    private RectTransform _rect;

    void Awake()
    {
        Instance = this;
        _rect = GetComponent<RectTransform>();
        
        _parentCanvas = GetComponent<Canvas>();
        
        Hide();
    }

    void Update()
    {
        if (!_tooltipPanel.activeSelf) return;

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Check if Canvas is Camera Space or Overlay
        if (_parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            _rect.position = mousePos;
        }
        else // Screen Space - Camera
        {
            // Convert Screen Point to Local Point inside the RectTransform
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform, 
                mousePos, 
                _parentCanvas.worldCamera, 
                out Vector2 localPoint
            );

            // Apply the local position
            _rect.anchoredPosition = localPoint;
        }
    }

    public void Show(PuckData data)
    {
        _tooltipPanel.SetActive(true);
        _nameText.text = data.puckName;
        _descriptionText.text = data.description;
        _statShowText.text = PuckDataFormatter.Format( data); // Example
        
        // Move it immediately so it doesn't "jump" from the last position
        UpdatePosition();
    }

    public void Hide()
    {
        _tooltipPanel.SetActive(false);
    }
}
