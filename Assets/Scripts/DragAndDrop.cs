using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropIntoLayout : MonoBehaviour, IDropHandler
{
    public GameObject dropZone;
    public void OnDrop(PointerEventData eventData)
    {
        GameObject dragged = DraggableElement.CurrentDraggedObject;
        if (name == "ScrollPanel")
        {
            if (dragged != null)
                if (dragged.GetComponent<DraggableElement>() != null)
                {
                    dragged.transform.SetParent(dropZone.transform, true);
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