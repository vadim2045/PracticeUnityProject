using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class DraggableElement : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public static GameObject CurrentDraggedObject { get; private set; }
    private RectTransform rectTransform;
    private Image image;
    private Transform originalParent;
    private int originalSiblingIndex;
    public GameObject mainCanvas;
    private GameObject obj;

    private void Awake()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var variables = GetComponent<Variables>();
        if ((bool)variables.declarations.GetDeclaration("Clone").value)
            obj = gameObject;
        else
        {
            obj = Instantiate(gameObject);
            obj.transform.position = transform.position;

            obj.GetComponent<Variables>().declarations.GetDeclaration("Clone").value = true;
            
        }
        rectTransform = obj.GetComponent<RectTransform>();
        image = obj.GetComponent<Image>();
        CurrentDraggedObject = obj;

        originalParent = obj.transform.parent;
        originalSiblingIndex = obj.transform.GetSiblingIndex();
        obj.transform.SetParent(mainCanvas.transform);
        image.color = new Color(0f, 255f, 200f, 0.7f);
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta * 1080 / Screen.height;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.color = new Color(255f, 255f, 255f, 1f);
        image.raycastTarget = true;
    }
}
