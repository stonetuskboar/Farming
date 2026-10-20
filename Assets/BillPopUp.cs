using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BillPopUp : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
{
    public Bill bill;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameDataManager.Instance.PlayAudio("Popup");
        Vector2 vec = bill.OgAnchorPosition;
        vec.y = 0;
        bill.DoAnchoredPositionTween(vec, 0.4f,type:EaseType.M3Spring);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bill.DoAnchoredPositionTween(bill.OgAnchorPosition, 0.5f, type: EaseType.M3Spring);
    }
}
