using UnityEngine;
using UnityEngine.EventSystems;

public class UIPuckHome : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        UIPuckItem puckItem = droppedObject.GetComponent<UIPuckItem>();

        if (puckItem != null)
        {
            // 1. Visuals: Return to the inventory layout
            puckItem.transform.SetParent(this.transform);
            
            //Debug.Log("Puck returned to Inventory.");
        }
    }
}
