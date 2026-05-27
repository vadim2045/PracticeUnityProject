using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class RobotMovement : MonoBehaviour
{
    enum Rotation
    {
        Down, Left, Up, Right
    };
    enum Action
    {
        move, turn, interact, repeat
    };
    bool done = false;
    int repeatCount;
    Action repeatAction;

    Rotation rotation = Rotation.Right;
    public List<string> Robotics { get; private set; } = new List<string>();
    public bool isMoving = true;
    private float moveDistance = 27.2f;

    void Move()
    {
        if (rotation == Rotation.Down)
        {
            transform.Translate(Vector3.down * moveDistance);
        }
        if (rotation == Rotation.Left)
        {
            transform.Translate(Vector3.left * moveDistance);
        }
        if (rotation == Rotation.Up)
        {
            transform.Translate(Vector3.up * moveDistance);
        }
        if (rotation == Rotation.Right)
        {
            transform.Translate(Vector3.right * moveDistance);
        }
    }
    void Turn(string act)
    {
        if (act.Split(' ')[1] == "налево")
        {
            rotation = (Rotation)(((int)rotation - 1) % 4);
        }
        else
        {
            rotation = (Rotation)(((int)rotation + 1) % 4);
        }
    }

    void Act(string act)
    {
        //Debug.Log("ПОВОРАЧИВАЮ" + act);
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

    int j = 0;
    void Update()
    {
        done = false;
    }
    void FixedUpdate()
    {
        bool error = false;
        if (!done)
        {
            //Debug.Log(((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value));
            if (((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Trim() != "")
            {
                Robotics = ((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Split(';').ToList<string>();
                Robotics.RemoveAt(Robotics.Count - 1);
                foreach (var s in Robotics)
                    Debug.Log(s);
            }
            else
            {
                error = true;
                j = 0;
                Robotics = new List<string>();
                //Debug.Log(Robotics.Count);
            }
            if ((Robotics.Count > 0) || !error)
            {
                if (j + 1 == Robotics.Count)
                    j = 0;
                if (Robotics[j] == "Идти" || Robotics[j].Split(' ')[0] == "Повернуть" || Robotics[j] == "Поднять/положить")
                    Act(Robotics[j]);
                else if (Robotics[j].Split(' ')[0] == "Повторить")
                {
                    if (Robotics.Count != j + 1)
                    {
                        for (int i = 0; i < int.Parse(Robotics[j].Split(' ')[1]); i++)
                        {
                            Act(Robotics[j + 1]);
                        }
                        j++;
                    }
                }
                j++;
                done = true;
            }
        }
    }
}

