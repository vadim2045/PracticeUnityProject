using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AlgSave : MonoBehaviour
{
    public GameObject RoboticsParent;
    public GameObject dropZone;
    public GameObject oneTimeSwitch;
    public GameObject moveBlock;
    public GameObject rotateBlock;
    public GameObject repeatBlock;
    public GameObject pickBlock;
    string actionGlobal;
    static public int Robot_ID = 0; // NEW ROBOT ID
    public void getList()
    {
        actionGlobal = "";
        for (int i = 0; i < dropZone.transform.childCount; i++)
        {
            Transform T = dropZone.transform.GetChild(i);
            string action = T.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text;
            if (action == "Повернуть")
            {
                action += " " + T.transform.Find("Dropdown").Find("Label").GetComponent<TextMeshProUGUI>().text + ';';
            }
            else if (action == "Повторить")
            {
                action += " " + T.transform.Find("InputField").Find("Text (Legacy)").GetComponent<Text>().text + ";";
            }
            else
            {
                action += ";";
            }
            actionGlobal += action;
        }
        for (int i = 0; i < RoboticsParent.transform.childCount; i++)
        {
            if ((int)(RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robot_ID").value) == Robot_ID)
            {
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robotics").value = actionGlobal;
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Loaded").value = true;
            }
        }
    }
    public void clearList()
    {
        actionGlobal = ";";
        for (int i = dropZone.transform.childCount - 1; i >= 0; i--)
        {
            Transform T = dropZone.transform.GetChild(i);
            Destroy(T.GameObject());
        }
        for (int i = 0; i < RoboticsParent.transform.childCount; i++)
        {
            if ((int)(RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robot_ID").value) == Robot_ID)
            {
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robotics").value = actionGlobal;
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Loaded").value = false;
            }
        }
    }
    public void switchList()
    {
        transform.Rotate(0, 0, 180);
        for (int i = 0; i < RoboticsParent.transform.childCount; i++)
        {
            if ((int)(RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robot_ID").value) == Robot_ID)
            {
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("oneTime").value = !(bool)RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("oneTime").value;
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Loaded").value = false;
            }
        }
    }

    public void SetRobotID()
    {
        Robot_ID = int.Parse(transform.Find("Text_Robot").GetComponent<TextMeshProUGUI>().text.Split(' ')[1]) - 1;
        Debug.Log(Robot_ID);
        for (int i = dropZone.transform.childCount - 1; i >= 0; i--) //очистка дропзоны
        {
            Transform T = dropZone.transform.GetChild(i);
            Destroy(T.GameObject());
        }

        for (int i = 0; i < RoboticsParent.transform.childCount; i++)
        {
            if ((int)(RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robot_ID").value) == Robot_ID)
            {
                if ((bool)RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("oneTime").value) //поворот switch в нужную сторону
                    oneTimeSwitch.transform.localEulerAngles = new Vector3(0, 0, 90);
                else oneTimeSwitch.transform.localEulerAngles = new Vector3(0, 0, 270);

                actionGlobal = (string)RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robotics").value;
                List<string> Robotics = new List<string>();
                if (actionGlobal != ";")
                    Robotics = ((string)RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Split(';').ToList<string>(); //загрузка в дропзону списка нужных команд
                if (Robotics.Count > 0)
                {
                    foreach (string s in Robotics)
                    {
                        GameObject copy = null;
                        if (s == "Идти")
                        {
                            copy = Instantiate(moveBlock);
                        }
                        else if (s.Split(' ')[0] == "Повернуть")
                        {
                            copy = Instantiate(rotateBlock);
                            copy.transform.Find("Dropdown").GetComponent<Image>().raycastTarget = true;
                            if (s.Split(' ')[1] == "налево")
                                copy.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().value = 0;
                            else copy.transform.Find("Dropdown").GetComponent<TMP_Dropdown>().value = 1;
                        }
                        else if (s == "Поднять/положить")
                        {
                            copy = Instantiate(pickBlock);
                        }
                        else if (s.Split(' ')[0] == "Повторить")
                        {
                            copy = Instantiate(repeatBlock);
                            copy.transform.Find("InputField").GetComponent<Image>().raycastTarget = true;
                            copy.transform.Find("InputField").GetComponent<InputField>().text = s.Split(' ')[1];
                        }
                        if (s.Trim() != "")
                        {
                            copy.GetComponent<Variables>().declarations.GetDeclaration("Clone").value = true;
                            copy.transform.SetParent(dropZone.transform, true);
                        }
                    }
                }
            }
        }
    }
}