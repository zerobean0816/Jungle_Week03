using UnityEngine;
using TMPro;

public class UIPuckHUD : MonoBehaviour
{
    [SerializeField] private GameObject _puckHUDItemPrefab; // simple prefab, just image + text

    private PuckHandler _puckHandler;

    void Start()
    {
        _puckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
        _puckHandler.OnPucksChanged += RefreshHUD;

        RefreshHUD(); // build initial state
    }

    void OnDestroy()
    {
        if (_puckHandler == null) return;
        _puckHandler.OnPucksChanged -= RefreshHUD;
    }

    void RefreshHUD()
    {
        // Clear old
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        if (_puckHandler.GetEquippedPucks() == null)return;

        // Rebuild
        foreach (PuckData puck in _puckHandler.GetEquippedPucks())
        {
            GameObject newItem = Instantiate(_puckHUDItemPrefab, transform);
            newItem.transform.localPosition = Vector3.zero;

            UIPuckItem uiItem = newItem.GetComponent<UIPuckItem>();
            uiItem.Canvas = GetComponentInParent<Canvas>().gameObject;
            uiItem.Init(puck);
        }
    }
}
