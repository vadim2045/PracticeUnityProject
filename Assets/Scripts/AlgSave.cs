using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AlgSave : MonoBehaviour
{
    public GameObject RoboticsParent;
    public GameObject dropZone;
    string actionGlobal;
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
            if ((int)(RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robot_ID").value) == 0)
            {
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Robotics").value = actionGlobal;
                RoboticsParent.transform.GetChild(i).GetComponent<Variables>().declarations.GetDeclaration("Loaded").value = true;
            }
        }
    }
}