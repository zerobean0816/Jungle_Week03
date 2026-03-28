using UnityEngine;
using UnityEngine.EventSystems;

public class UIPuckSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] int SlotIndex; // 슬롯 인덱스 (0, 1, 2)
    [SerializeField] private GameObject puckHome; // 슬롯이 비어있을 때 보이는 시각적 요소
    PuckHandler _playerPuckHandler;
    
    private bool _isEquitted => transform.childCount >  0; // 슬롯에 퍽이 장착되어 있는지 여부

    private void Start()
    {
        _playerPuckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
    }

    public void OnDrop(PointerEventData eventData)
    {
       // Debug.Log("Dropped on slot " + slotIndex);

        GameObject droppedObject = eventData.pointerDrag;
        UIPuckItem newItem = droppedObject.GetComponent<UIPuckItem>();

        if (newItem != null)
        {
            if (_isEquitted)
            {
                // 이미 장착된 퍽이 있다면, 해당 퍽을 인벤토리로 되돌리는 로직
                Transform existingPuck = transform.GetChild(0); // 슬롯에 있는 기존 퍽
                existingPuck.SetParent(puckHome.transform); // 인벤토리로 이동
                existingPuck.localPosition = Vector3.zero; // 인벤토리 내에서 위치 초기화
            }
            // 퍽 데이터 가져오기
            newItem.transform.SetParent(transform);
            newItem.transform.localPosition = Vector3.zero; // 슬롯 중앙에 위치

            if (_playerPuckHandler!= null)
            {
                _playerPuckHandler.EquiedPuck(SlotIndex, newItem.Puckdata); // 퍽 장착 처리
                _playerPuckHandler.RecalculateChangedPucks(); // 변경된 퍽에 따른 스탯 재계산
            }
        }
    }

    public int GetSlotIndex()
    {
        return SlotIndex;
    }
}
