using UnityEngine;
using UnityEngine.EventSystems;

public class UIPuckItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public PuckData Puckdata;
    public GameObject Canvas;


    private PuckHandler _playerPuckHandler;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;

    public void Start()
    {
        if (GameManager.Instance.Player == null)
        {
            Debug.LogError("[UIPuckItem] : Player GameObject not found in the scene.");
        }

        _playerPuckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
    }

    public void Init()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalParent = transform.parent; // 원래 부모 저장

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent; // 원래 부모 저장

        UIPuckSlot sourceSlot = _originalParent.GetComponent<UIPuckSlot>();
        if (sourceSlot != null)
        {
            _playerPuckHandler.EquiedPuck(sourceSlot.GetSlotIndex(), null);
        }

        if (Canvas != null)
        {
            transform.SetParent(Canvas.transform);
            transform.SetAsLastSibling(); // This forces it to the bottom of the list (TOP of the screen)
        }

        _canvasGroup.blocksRaycasts = false; // 드래그 중에는 레이캐스트 차단 해제
        _canvasGroup.alpha = 0.6f; // 드래그 중에는 반투명 처리
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
}
