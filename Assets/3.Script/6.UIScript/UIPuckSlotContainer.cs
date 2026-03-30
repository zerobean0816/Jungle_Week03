using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIPuckSlotContainer : MonoBehaviour, IDropHandler
{

    
    private PuckHandler _puckHandler;
    private PlayerStat _playerStat;
    
    private float _textApplearDuratoin = 1.5f;
    private float _floatDistance = 1f;
    private Vector3 _startTextPosition;

    [SerializeField] TextMeshProUGUI _statText;
    [SerializeField] private GameObject _puckItemPrefab;
    [SerializeField] Transform ReturnTransform;

    void Awake() // ← was Start, Awake runs earlier
    {
        _puckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
        _playerStat = GameManager.Instance.Player.GetComponent<PlayerStat>();
    }

    void Start()
    {
        GameManager.Instance.OnPuckPaused += RefreshUI; // ← event subscription stays in Start
        _statText.gameObject.SetActive(false);
        _startTextPosition = _statText.rectTransform.anchoredPosition;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnPuckPaused -= RefreshUI;
    }

    public void RefreshUI()
    {
        if (_puckItemPrefab == null)
        {
            Debug.LogError("[UIPuckSlotContainer] _puckItemPrefab is not assigned!");
            return;
        }

        if (_puckHandler == null)
        {
            Debug.LogError("[UIPuckSlotContainer] _puckHandler is null!");
            return;
        }

        foreach (Transform child in transform)
        {
            if (child.gameObject == _statText.gameObject) continue; // ← skip statText
            Destroy(child.gameObject);
        }

        foreach (PuckData puck in _puckHandler.GetEquippedPucks())
        {
            GameObject newItem = Instantiate(_puckItemPrefab, this.transform);
            newItem.transform.localPosition = Vector3.zero;

            UIPuckItem uiItem = newItem.GetComponent<UIPuckItem>();
            uiItem.Canvas = GetComponentInParent<Canvas>().gameObject;
            uiItem.Init(puck);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIPuckItem puckItem = eventData.pointerDrag.GetComponent<UIPuckItem>();
        if (puckItem == null) return;

        if (GameManager.Instance.PuckCounts < _puckHandler.MaxListCount 
        && GameManager.Instance.PuckPoints >= puckItem.Puckdata.PointCost)
        {
            puckItem.transform.SetParent(this.transform);
            puckItem.transform.localPosition = Vector3.zero;

            _puckHandler.EquipPuck(puckItem.Puckdata);
            _playerStat.PreviewModifiers(puckItem.Puckdata.modifiers);
            return;
        }
        else if (GameManager.Instance.PuckPoints < puckItem.Puckdata.PointCost)
        {
            _statText.text = "포인트 부족!.. ( " +  puckItem.Puckdata.PointCost + " ) 필요";
            StartCoroutine(AnimateAndReset(Color.red));
        }

            puckItem.transform.SetParent(ReturnTransform);
            puckItem.transform.localPosition = Vector3.zero;
    }


    private IEnumerator AnimateAndReset(Color color)
    {
        _statText.gameObject.SetActive(true);
        _statText.color = color;

        float elapsed = 0f;
        Color startColor = color;
        Vector2 startPos = _statText.rectTransform.anchoredPosition; // ← snapshot position

        yield return new WaitForSecondsRealtime(0.3f);

        while (elapsed < _textApplearDuratoin)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / _textApplearDuratoin;

            // Use anchoredPosition for UI elements, not world position
            _statText.rectTransform.anchoredPosition = startPos + new Vector2(0, t * _floatDistance);
            _statText.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }

        _statText.color = startColor;
        _statText.rectTransform.anchoredPosition = _startTextPosition; // ← restore value
        _statText.gameObject.SetActive(false);
    }
}
