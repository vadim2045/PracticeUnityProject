using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropIntoLayout : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        //GameObject dragged = eventData.pointerDrag.gameObject;
        GameObject dragged = DraggableElement.CurrentDraggedObject;
        if (name == "DropZoneList")
        {
            if (dragged != null)
                if (dragged.GetComponent<DraggableElement>() != null)
                {
                    dragged.transform.SetParent(transform, true);
                    if (dragged.transform.Find("Dropdown") != null)
                        dragged.transform.Find("Dropdown").GetComponent<Image>().raycastTarget = true;
                    if (dragged.transform.Find("InputField") != null)
                        dragged.transform.Find("InputField").GetComponent<Image>().raycastTarget = true;
                }
        }
        else
        {
            Destroy(dragged);
        }

    }
}