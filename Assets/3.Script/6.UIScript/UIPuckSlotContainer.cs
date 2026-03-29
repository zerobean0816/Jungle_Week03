using UnityEngine;
using UnityEngine.EventSystems;

public class UIPuckSlotContainer : MonoBehaviour, IDropHandler
{
    private PuckHandler _puckHandler;
    private PlayerStat _playerStat;

    [SerializeField] Transform ReturnTransform;

    void Start()
    {
        _puckHandler = GameManager.Instance.Player.GetComponent<PuckHandler>();
        _playerStat = GameManager.Instance.Player.GetComponent<PlayerStat>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        UIPuckItem puckItem = eventData.pointerDrag.GetComponent<UIPuckItem>();
        if (puckItem == null) return;

        if (GameManager.Instance.PuckCounts < _puckHandler.MaxListCount)
        {
            puckItem.transform.SetParent(this.transform);
            puckItem.transform.localPosition = Vector3.zero;

            _puckHandler?.EquipPuck(puckItem.Puckdata);

            _playerStat?.PreviewModifiers(puckItem.Puckdata.modifiers);
            
            Debug.Log("[UIPuckSlotContainer] : Puck Equipped Successfully.");
        }
        else
        {
            puckItem.transform.SetParent(ReturnTransform);
            puckItem.transform.localPosition = Vector3.zero;
            
            Debug.Log("Slot is Full! Returning puck.");
        }
    }
}
