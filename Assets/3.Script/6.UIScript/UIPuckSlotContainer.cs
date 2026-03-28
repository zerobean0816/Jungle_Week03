using UnityEngine;
using UnityEngine.EventSystems;

public class UIPuckSlotContainer : MonoBehaviour, IDropHandler
{
    private PuckHandler _puckHandler;

    [SerializeField] Transform ReturnTransform;

    void Start()
    {
        _puckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIPuckItem puckItem = eventData.pointerDrag.GetComponent<UIPuckItem>();
        if (puckItem == null) return;

        // 장착 가능한 빈 자리가 있는지 확인
        if (GameManager.Instance.PuckCounts < _puckHandler.MaxListCount)
        {
            // 1. 성공: 슬롯에 자식으로 등록
            puckItem.transform.SetParent(this.transform);
            puckItem.transform.localPosition = Vector3.zero;

            _puckHandler?.EquipPuck(puckItem.Puckdata);
            _puckHandler?.RecalculateChangedPucks();
            
            Debug.Log("Puck Equipped Successfully.");
        }
        else
        {
            // 2. 실패: 자리가 없으므로 원래 위치(ReturnTransform)로 복귀
            puckItem.transform.SetParent(ReturnTransform);
            puckItem.transform.localPosition = Vector3.zero;
            
            Debug.Log("Slot is Full! Returning puck.");
        }
    }
}
