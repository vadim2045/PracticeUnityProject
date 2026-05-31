using System.Drawing;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class InfoScreen : MonoBehaviour, IPointerExitHandler
{
    //public int n;
    public void Enter(GameObject obj)
    {
        int n = (int)obj.GetComponent<Variables>().declarations.GetDeclaration("n").value;
        Debug.Log("Clicked");
        transform.position = obj.transform.position;

        //RobotMovement.actionPoints[n].
        Transform text1 = transform.Find("Canvas1").Find("Text1");
        Transform item1 = transform.Find("Item1");
        if (n >= 22 && n <= 25) // For unlock room with 1 resource
        {
            int max = (n - 21) * 5;
            text1.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current1.ToString() + "/" + max.ToString();
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp1]);
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
        else if (n == 26 || n == 27) // For unlock room with 2-3 resource
        {
            Transform text2 = transform.Find("Canvas2").Find("Text2");
            Transform item2 = transform.Find("Item2");
            int max1 = 10;
            int max2 = 20;

            text1.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current1.ToString() + "/" + max1.ToString();
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp1]);
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;

            text2.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current2.ToString() + "/" + max2.ToString();
            Sprite newSprite1 = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp2]);
            item2.GetComponent<SpriteRenderer>().sprite = newSprite1;

            if (n == 27) // For unlock room with 3 resource
            {
                Transform text3 = transform.Find("Canvas3").Find("Text3");
                Transform item3 = transform.Find("Item3");
                max1 = 20;
                max2 = 10;
                int max3 = 30;
                text1.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current1.ToString() + "/" + max1.ToString();
                newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp1]);
                item1.GetComponent<SpriteRenderer>().sprite = newSprite;
                text2.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current2.ToString() + "/" + max2.ToString();
                newSprite1 = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp2]);
                item2.GetComponent<SpriteRenderer>().sprite = newSprite1;

                text3.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current3.ToString() + "/" + max3.ToString();
                Sprite newSprite3 = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp3]);
                item3.GetComponent<SpriteRenderer>().sprite = newSprite3;
            }
        }
        else if (n >= 0 && n <= 5)
        {
            text1.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current1.ToString();
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/" + RobotMovement.Images[RobotMovement.actionPoints[n].id_inp1]);
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
            transform.position = obj.transform.position + new Vector3(0, -30f, 0);
        }
        else if (n == 20)
        {
            text1 = transform.Find("Canvas2").Find("Canvas1").Find("Text1");
            Transform text2 = transform.Find("Canvas2").Find("Canvas2").Find("Text2");
            text1.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current1.ToString() + "/5";
            text2.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current2.ToString() + "/3";
            transform.position = obj.transform.position + new Vector3(0, -26f, 0);
        }
        else if (n == 21)
        {
            text1 = transform.Find("Canvas2").Find("Canvas1").Find("Text1");
            text1.GetComponent<TextMeshProUGUI>().text = RobotMovement.actionPoints[n].current1.ToString() + "/25";
            transform.position = obj.transform.position + new Vector3(-81f, 0, 0);
        }


    }

    public void HelpButton(GameObject obj)
    {
        int n = (int)obj.GetComponent<Variables>().declarations.GetDeclaration("n").value;
        Debug.Log("Clicked");
        EventSystem.current.SetSelectedGameObject(null);
        Transform item1 = transform.Find("Item_1");
        Transform item2 = transform.Find("Item_2");
        if (n == 0)
        {
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Iron_ore");
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
            newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Iron_ingot");
            item2.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
        else if (n == 1)
        {
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Copper_ore");
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
            newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Copper_ingot");
            item2.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
        else if (n == 2)
        {
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Iron_ingot");
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
            newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Iron_plate");
            item2.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
        else if (n == 3)
        {
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Iron_plate");
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
            newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Iron_pipe");
            item2.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
        else if (n == 4)
        {
            Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Copper_ingot");
            item1.GetComponent<SpriteRenderer>().sprite = newSprite;
            newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/Copper_wire");
            item2.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
        transform.position = obj.transform.position;
    }
    private Vector2 lastPointerPosition;
    private Camera lastEventCamera;
    public void OnPointerExit(PointerEventData eventData)
    {
        lastPointerPosition = eventData.position;
        lastEventCamera = eventData.pressEventCamera;
        Invoke(nameof(DelayedHideCheck), 0.05f);
    }

    private void DelayedHideCheck()
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(),
            lastPointerPosition,
            lastEventCamera))
        {
            transform.position = new Vector3(-1000, -1000, -1000);
            Debug.Log("Bye bye");
        }
    }

}
