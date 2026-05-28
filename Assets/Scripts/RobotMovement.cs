using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
    Rotation rotation = Rotation.Down;

    public List<string> Robotics { get; private set; } = new List<string>();
    public bool isMoving = true;
    private float moveDistance = 27.2f;
    Point startPoint = new Point(1, 1);

    // 0  — стен нет (можно идти везде)
    // 1  — стена сверху (нельзя Up)
    // 2  — стена справа (нельзя Right)
    // 4  — стена снизу (нельзя Down)
    // 8  — стена слева (нельзя Left)
    int[,] matrix =
    {
        { 15, 13, 15, 13, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 11, 0, 8, 0, 12, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 0, 8, 8, 8, 8, 8, 8, 8, 8, 8, 12, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 1, 0, 0, 0, 4, 1, 0, 0, 0, 4, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 3, 2, 0, 2, 6, 3, 2, 0, 2, 6, 15, 15, 15, 15, 15},

        { 15, 1, 0, 0, 4, 9, 8, 4, 15, 15, 15, 15, 1, 8, 12, 15, 15, 15, 15, 15 },
        { 15, 1, 0, 0, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 3, 2, 0, 4, 3, 2, 4, 15, 15, 15, 15, 1, 2, 6, 15, 15, 15, 15, 15},

        { 15, 15, 15, 1, 4, 9, 8, 4, 15, 15, 15, 15, 1, 8, 12, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 3, 2, 6, 15, 15, 15, 15, 3, 2, 6, 15, 15, 15, 15, 15},

        { 15, 15, 15, 1, 4, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 9, 8, 8, 9, 4, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 3, 2, 2, 2, 6, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15}
    }
    ;
    int newRotation(Rotation rot)
    {
        switch (rot)
        {
            case Rotation.Up:
                return 1;
            case Rotation.Right:
                return 2;
            case Rotation.Down:
                return 4;
            case Rotation.Left:
                return 8;
        }
        return 0;
    }


    void Move()
    {
        bool canMove = ((matrix[startPoint.X, startPoint.Y] & newRotation(rotation)) == 0);
        if (canMove)
        {
            transform.Translate(Vector3.right * moveDistance);
            switch (rotation)
            {
                case Rotation.Down:
                    startPoint.Y += 1;
                    break;
                case Rotation.Up:
                    startPoint.Y -= 1;
                    break;
                case Rotation.Right:
                    startPoint.X += 1;
                    break;
                case Rotation.Left:
                    startPoint.X -= 1;
                    break;
            }
        }
        else
        {
            Debug.Log("CANT MOVE");
            // остановка робота
        }
        Debug.Log(startPoint);
    }
    void Turn(string act)
    {
        if (act.Split(' ')[1] == "налево")
        {
            rotation = (Rotation)(((int)rotation - 1 + 4) % 4);
            transform.Rotate(0, 0, 90);
        }
        else
        {
            rotation = (Rotation)(((int)rotation + 1 + 4) % 4);
            transform.Rotate(0, 0, -90);
        }
        //Debug.Log(rotation);
    }

    void Act(string act)
    {
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
                // ОСТАНОВКА РОБОТА
            }
        if (Robotics.Count > 0)
        {
            if (Robotics.First() == "Идти" || Robotics.First().Split(' ')[0] == "Повернуть" || Robotics.First() == "Поднять/положить")
                Act(Robotics.First());

            else if (Robotics.First().Split(' ')[0] == "Повторить")
            {
                if (Robotics.Count > 1 && Robotics.First().Split(' ')[1] != "")
                {
                    int n = int.Parse(Robotics.First().Split(' ')[1]);

                    Robotics.RemoveAt(0);

                    for (int i = 0; i < n; i++)
                    {
                        Robotics.Insert(0, Robotics[0]);
                    }

                }
                else
                {
                    // Ошибка пользователю (цикл) и остановка робота
                }
            }
            Robotics.RemoveAt(0);
        }
    }
}