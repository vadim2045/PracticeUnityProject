using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class RobotMovement : MonoBehaviour
{
    public List<string> Robotics { get; private set; } = new List<string>();
    public bool isMoving = true;
    private float moveDistance = 27.2f;

    void Move()
    {
        transform.Translate(Vector3.right * moveDistance);
    }
    void Turn(string act)
    {
        if (act.Split(' ')[1] == "налево")
        {
            transform.Rotate(0, 0, (transform.rotation.z + 90f + 360f) % 360f);
        }
        else
        {
            transform.Rotate(0, 0, (transform.rotation.z - 90f + 360f) % 360f);
        }
    }

    void Act(string act)
    {
        //Debug.Log(act);
        if (act == "Идти")
        {
            Move();
        }
        else if (act.Split(' ')[0] == "Повернуть")
        {
            Turn(act);
        }
        else if (act == "Поднять/положить")
        {
            Interact();
        }
    }

    void Interact()
    {

    }

    void FixedUpdate()
    {
        if (Robotics.Count == 0 && (bool)GetComponent<Variables>().declarations.GetDeclaration("Loaded").value)
            if (((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Trim() != "")
            {
                Robotics = ((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Split(';').ToList<string>();
            }
            else
            {
                Robotics = new List<string>();
                GetComponent<Variables>().declarations.GetDeclaration("Loaded").value = false;
            }
        if (Robotics.Count > 0)
        {
            if (Robotics.First() == "Идти" || Robotics.First().Split(' ')[0] == "Повернуть" || Robotics.First() == "Поднять/положить")
                Act(Robotics.First());

            else if (Robotics.First().Split(' ')[0] == "Повторить")
            {
                if (Robotics.Count > 1)
                {
                    int n = int.Parse(Robotics.First().Split(' ')[1]);
                    Robotics.RemoveAt(0);
                    for (int i = 0; i < n; i++)
                    {
                        Robotics.Insert(0, Robotics[0]);
                    }

                }
            }
            Robotics.RemoveAt(0);
        }
    }
}