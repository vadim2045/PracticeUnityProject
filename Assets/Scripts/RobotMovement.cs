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

    struct ActionPoints
    {
        Point coords;
        string name;
        int id_inp1;
        int id_inp2;
        int id_inp3;
        int id_out;
        public ActionPoints(Point coords, string name, int id_out, int id_inp1 = -1, int id_inp2 = -1, int id_inp3 = -1)
        {
            this.coords = coords;
            this.name = name;
            this.id_out = id_out;
            this.id_inp1 = id_inp1;
            this.id_inp2 = id_inp2;
            this.id_inp3 = id_inp3;
            
        }
    }
    struct storage
    {
        int id;
        int current;
    }
    enum Resources
    {
        Error = -1,
        Nothing = 0,
        Iron_ore = 1,
        Iron_ingot = 2,
        Iron_plate = 3,
        Iron_pipe = 4,
        Copper_ore = 5,
        Copper_Ingot = 6,
        Copper_wire = 7,
        Microchip = 8
    };


    ActionPoints[] actionPoints = // Присутствует возможность класть\брать предметы черзе стены, это не баг, а фича!!! (iron plate, iron ingot, copper wire)
    {
        new ActionPoints(new Point (0, 0), "Iron ore", 1, 1),
        new ActionPoints(new Point (0, 2), "Copper ore", 5, 5),

        new ActionPoints(new Point (1, 7), "Iron furnace input", 0, 1),
        new ActionPoints(new Point (1, 9), "Iron furnace output", 2),

        new ActionPoints(new Point (1, 12), "Copper  furnace input", 0, 5),
        new ActionPoints(new Point (1, 14), "Copper  furnace output", 6),
        
        new ActionPoints(new Point (2, 0), "Iron ingot crate", 2, 2),
        new ActionPoints(new Point (3, 0), "Copper ingot crate", 6, 6),
        new ActionPoints(new Point (4, 0), "Iron plate crate", 3, 3),
        new ActionPoints(new Point (5, 0), "Copper wire crate", 7, 7),
        new ActionPoints(new Point (6, 0), "Iron pipe crate", 4, 4),
        new ActionPoints(new Point (7, 0), "Microchip crate", 8, 8),

        new ActionPoints(new Point (11, 2), "Monitor room input", 0, 3, 7),

        new ActionPoints(new Point (18, 3), "Energy station input", 0, 8),

        new ActionPoints(new Point (5, 8), "Iron plate input", 0, 2),
        new ActionPoints(new Point (9, 8), "Iron plate output", 3),

        new ActionPoints(new Point (10, 8), "Iron pipe input", 0, 3),
        new ActionPoints(new Point (15, 8), "Iron pipe output", 4),

        new ActionPoints(new Point (5, 11), "Copper wire input", 0, 6),
        new ActionPoints(new Point (9, 11), "Copper wire output", 7),

        new ActionPoints(new Point (10, 11), "Microchip input", 0, 3, 7),
        new ActionPoints(new Point (14, 11), "Microchip output", 8),

        new ActionPoints(new Point (2, 5), "Iron furnace unlock", 0, 1),
        new ActionPoints(new Point (5, 7), "Iron plate unlock", 0, 2),
        new ActionPoints(new Point (10, 7), "Iron pipe unlock", 0, 3),
        new ActionPoints(new Point (2, 10), "Copper furnace unlock", 0, 2),
        new ActionPoints(new Point (5, 12), "Copper wire unlock", 0, 3, 6),
        new ActionPoints(new Point (10, 12), "Microchip unlock", 0, 3, 4, 7)
    };
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