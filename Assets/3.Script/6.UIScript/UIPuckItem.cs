using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIPuckItem : MonoBehaviour, 
    IDragHandler, IBeginDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    public PuckData Puckdata;
    public GameObject Canvas;
    public Image Image;
    public int OccupiedStartIndex = -1; // -1 = 인벤토리에 있음
    [SerializeField] private TextMeshProUGUI _cardName;

    private PuckHandler _playerPuckHandler;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;

    public void Start()
    {   
        // Check if player is null
        CheckPlayerReference();

        _playerPuckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
        _canvasGroup = GetComponent<CanvasGroup>();

        ChangePuckItemName();

        ChangeColorByType();
    }

    public void Init(PuckData data)
    {
        Puckdata = data;
        _originalParent = transform.parent;
        
        _playerPuckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
        _canvasGroup = GetComponent<CanvasGroup>();

        ChangePuckItemName();
        ChangeColorByType();
    }

    void CheckPlayerReference()
    {
        if (GameManager.Instance.Player == null)
        {
            Debug.LogError("[UIPuckItem] : Player GameObject not found in the scene.");
        }
    }

    void ChangePuckItemName()
    {
        // Set Puck Name By its puck
        if (_cardName != null && Puckdata != null)
        {
            _cardName.text = Puckdata.puckName;
        }
        else
        {
            Debug.LogWarning($"[UIPuckItem] : {gameObject.name}의 Text나 Data가 비어있습니다.");
        }
    }

    void ChangeColorByType()
    {
        if (Puckdata._isNegative)
        {
            Image.color = Color.red;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Check if we are being dragged OUT of a Slot
        UIPuckSlotContainer sourceSlot = transform.parent.GetComponent<UIPuckSlotContainer>();
        if (sourceSlot != null)
        {
            // Tell the handler to remove this puck data
            _playerPuckHandler?.UnequipPuck(this.Puckdata); 
            _playerPuckHandler?.RecalculateChangedPucks();
        }

        // Existing Canvas logic...
        if (Canvas != null)
        {
            transform.SetParent(Canvas.transform);
            transform.SetAsLastSibling(); 
        }

        transform.SetParent(Canvas.transform);
        transform.SetAsLastSibling();

        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.alpha = 0.6f;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // 드래그 중에는 마우스 위치로 이동
        // 드래그 중 처리
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        if (transform.parent == Canvas.transform)
        {
            // If no slot caught us, return to our last valid home (Inventory or Slot)
            transform.SetParent(_originalParent);
        }

        // This makes it "snap" to the center of the Slot or the Layout Group in Home
        transform.localPosition = Vector3.zero;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipUI.Instance.Show(Puckdata);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipUI.Instance.Hide();
    }
}
