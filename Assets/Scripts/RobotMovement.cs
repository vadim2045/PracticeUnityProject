using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class RobotMovement : MonoBehaviour
{
    enum Rotation
    {
        Down, Left, Up, Right
    };
    Rotation rotation = Rotation.Down;

    public struct ActionPoints
    {
        public Point coords;
        public string name;
        public int id_inp1;
        public int id_inp2;
        public int id_inp3;
        public int id_out;
        public int current1;
        public int current2;
        public int current3;
        public ActionPoints(Point coords, string name, int id_out, int id_inp1 = -1, int id_inp2 = -1, int id_inp3 = -1, int current1 = 0, int current2 = 0, int current3 = 0)
        {
            this.coords = coords;
            this.name = name;
            this.id_out = id_out;
            this.id_inp1 = id_inp1;
            this.id_inp2 = id_inp2;
            this.id_inp3 = id_inp3;
            this.current1 = current1;
            this.current2 = current2;
            this.current3 = current3;
        }
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
    static public ActionPoints[] actionPoints = // Присутствует возможность класть\брать предметы черзе стены, это не баг, а фича!!! (iron plate, iron ingot, copper wire)
    {
        new ActionPoints(new Point (2, 0), "Iron ingot crate", 2, 2), // 0
        new ActionPoints(new Point (3, 0), "Copper ingot crate", 6, 6),
        new ActionPoints(new Point (4, 0), "Iron plate crate", 3, 3),
        new ActionPoints(new Point (5, 0), "Copper wire crate", 7, 7),
        new ActionPoints(new Point (6, 0), "Iron pipe crate", 4, 4),
        new ActionPoints(new Point (7, 0), "Microchip crate", 8, 8),

        new ActionPoints(new Point (0, 0), "Iron ore", 1, 1, current1:10000000), //6
        new ActionPoints(new Point (0, 2), "Copper ore", 5, 5, current1:10000000),


        new ActionPoints(new Point (1, 7), "Iron furnace input", -1, 1), // 8
        new ActionPoints(new Point (1, 9), "Iron furnace output", 2),
        new ActionPoints(new Point (5, 8), "Iron plate input", -1, 2),
        new ActionPoints(new Point (9, 8), "Iron plate output", 3),
        new ActionPoints(new Point (10, 8), "Iron pipe input", -1, 3), // 12
        new ActionPoints(new Point (14, 8), "Iron pipe output", 4), // 13


        new ActionPoints(new Point (1, 12), "Copper  furnace input", -1, 5),
        new ActionPoints(new Point (1, 14), "Copper  furnace output", 6),
        new ActionPoints(new Point (5, 11), "Copper wire input", -1, 6),
        new ActionPoints(new Point (9, 11), "Copper wire output", 7), // 17


        new ActionPoints(new Point (10, 11), "Microchip input", -1, 3, 7), // 18
        new ActionPoints(new Point (14, 11), "Microchip output", 8),

        new ActionPoints(new Point (11, 2), "Monitor room input", -1, 3, 7), // 20

        new ActionPoints(new Point (18, 3), "Energy station input", -1, 8), // 21

        new ActionPoints(new Point (2, 5), "Iron furnace unlock", -1, 1), // 22
        new ActionPoints(new Point (5, 7), "Iron plate unlock", -1, 2), // 23
        new ActionPoints(new Point (10, 7), "Iron pipe unlock", -1, 3), // 24
        new ActionPoints(new Point (2, 10), "Copper furnace unlock", -1, 2), // 25
        new ActionPoints(new Point (5, 12), "Copper wire unlock", -1, 3, 6), // 26
        new ActionPoints(new Point (10, 12), "Microchip unlock", -1, 3, 4, 7) // 27
    };
    public List<string> Robotics { get; private set; } = new List<string>();
    public bool isMoving = true;
    private float moveDistance = 54f;
    Point startPoint = new Point(1, 1);
    public GameObject[] roomLocks;
    public GameObject[] tables;
    int[,] matrix =
    {
        // 0  — стен нет (можно идти везде)
        // 1  — стена сверху (нельзя Up)
        // 2  — стена справа (нельзя Right)
        // 4  — стена снизу (нельзя Down)
        // 8  — стена слева (нельзя Left)
        { 15, 13, 15, 13, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 11, 0, 8, 0, 12, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4/* Iron furnace room 0 = unlock, 4 = lock*/, 8, 8, 8, 8, 12/* Copper furnace room 8 = unlock, 12 = lock*/, 8, 8, 8, 8, 12, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 1, 0, 0, 0, 4, 1, 0, 0, 0, 4, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 3, 2, 2/* Iron plate room 0 = unlock, 2 = lock*/, 2, 6, 3, 2, 2/* Copper wire room 0 = unlock, 2 = lock*/, 2, 6, 15, 15, 15, 15, 15},

        { 15, 1, 0, 0, 4, 9, 8, 4, 15, 15, 15, 15, 1, 8, 12, 15, 15, 15, 15, 15 },
        { 15, 1, 0, 0, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 1, 0, 0, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 9, 0, 0, 0, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 3, 2, 2, 0, 4, 3, 2, 6/* Iron pipe room 4 = unlock, 6 = lock*/, 15, 15, 15, 15, 3/* Microchip room 1 = unlock, 3 = lock*/, 2, 6, 15, 15, 15, 15, 15},

        { 15, 15, 15, 1, 4, 9, 8, 4, 15, 15, 15, 15, 1, 8, 12, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 1, 0, 4, 15, 15, 15, 15, 1, 0, 4, 15, 15, 15, 15, 15},
        { 15, 15, 15, 1, 4, 3, 2, 6, 15, 15, 15, 15, 3, 2, 6, 15, 15, 15, 15, 15},

        { 15, 15, 15, 1, 4, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 9, 8, 8, 0, 4, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 3, 2, 2, 2, 6, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15},
        { 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15}
    };
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
            //Debug.Log("CANT MOVE");
            // остановка робота
        }
        //Debug.Log(startPoint);
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
        int ID_in_hand = (int)GetComponent<Variables>().declarations.GetDeclaration("Resource_ID").value;
        bool canMove = ((matrix[startPoint.X, startPoint.Y] & newRotation(rotation)) == 0);
        bool x = false, y = false;
        int k = 0;
        switch (rotation)
        {
            case Rotation.Left:
                x = true;
                k = -1;
                break;

            case Rotation.Up:
                y = true;
                k = -1;
                break;

            case Rotation.Right:
                x = true;
                k = 1;
                break;

            case Rotation.Down:
                y = true;
                k = 1;
                break;
        }
        foreach (ActionPoints point in actionPoints)
        {
            Point temp = new Point();
            if (y)
            {
                temp.X = startPoint.X;
                temp.Y = startPoint.Y + k;
            }
            else if (x)
            {
                temp.X = startPoint.X + k;
                temp.Y = startPoint.Y;
            }
            if (point.coords == temp)
            {
                //Debug.Log(point.name + point.current);
                // PICKUP
                if (ID_in_hand == 0 && point.id_out != -1)
                {
                    if (point.current1 > 0)
                    {
                        int n = Array.FindIndex(actionPoints, x => x.coords == point.coords);

                        //Debug.Log(actionPoints[n].name + " " + actionPoints[n].current1);
                        actionPoints[n].current1--;
                        //Debug.Log(actionPoints[n].name + " " + actionPoints[n].current1);

                        GetComponent<Variables>().declarations.GetDeclaration("Resource_ID").value = point.id_out;

                        Sprite newSprite = UnityEngine.Resources.Load<Sprite>("Sprites/" + Images[point.id_out]);
                        //Debug.Log(newSprite);


                        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = newSprite;
                        transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                        transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
                else if (ID_in_hand != 0 && (point.id_inp1 == ID_in_hand || point.id_inp2 == ID_in_hand || point.id_inp3 == ID_in_hand))
                {
                    int n = Array.FindIndex(actionPoints, x => x.coords == point.coords);
                    //Debug.Log(actionPoints[n].name + " " + actionPoints[n].current);
                    //Debug.Log(actionPoints[n + 1].name + " " + actionPoints[n + 1].current);
                    if (n == 8 || n == 10 || n == 12 || n == 14 || n == 16) // Для столов из 1 элемента
                    {
                        actionPoints[n + 1].current1++;
                        //Debug.Log(actionPoints[n + 1].current1 + actionPoints[n+1].name);
                    } 
                    else if (n == 18 || n == 20)
                    {
                        if (actionPoints[n].id_inp1 == ID_in_hand)
                        {
                            actionPoints[n].current1++;
                        }
                        else
                        {
                            actionPoints[n].current2++;
                        }
                        if (actionPoints[n].current1 >= 1 && actionPoints[n].current2 >= 2 && n == 18) // Microchips
                        {
                            actionPoints[n].current1 -= 1;
                            actionPoints[n].current2 -= 2;
                            actionPoints[n + 1].current1++;
                        }
                        else if (actionPoints[n].current1 >= 5 && actionPoints[n].current2 >= 3 && n == 20) // Robot
                        {
                            actionPoints[n].current1 -= 5;
                            actionPoints[n].current2 -= 3;
                            // ROBOT_SPAWN();
                        }
                    }
                    else if ((n >= 0 && n <= 7) || n == 21) // Crates 
                    {
                        actionPoints[n].current1++;
                        if (n == 21 && actionPoints[n].current1 >= 25)
                        {
                            // POBEDA(); !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        }
                    }
                    else if (n >= 22)
                    {
                        if (n <= 25)
                        {
                            actionPoints[n].current1++;
                            if (actionPoints[n].current1 >= ((n - 21) * 5))
                            {
                                // Unlock_room_N();
                                if (n == 22)
                                {
                                    matrix[actionPoints[n].coords.X,actionPoints[n].coords.Y-1] = 0;
                                    actionPoints[n].id_inp1 = -1;
                                    actionPoints[n].current1 = 0;
                                    roomLocks[0].GetComponent<SpriteRenderer>().enabled = false;
                                    roomLocks[0].GetComponent<Canvas>().enabled = false;

                                    roomLocks[1].GetComponent<Canvas>().enabled = true;
                                    roomLocks[3].GetComponent<Canvas>().enabled = true;

                                    tables[0].GetComponent<Canvas>().enabled = true;
                                }
                                else if (n == 25)
                                {
                                    matrix[actionPoints[n].coords.X, actionPoints[n].coords.Y - 1] = 8;
                                    actionPoints[n].id_inp1 = -1;
                                    actionPoints[n].current1 = 0;
                                    roomLocks[3].GetComponent<SpriteRenderer>().enabled = false;
                                    roomLocks[3].GetComponent<Canvas>().enabled = false;

                                    roomLocks[4].GetComponent<Canvas>().enabled = true;

                                    tables[3].GetComponent<Canvas>().enabled = true;
                                }
                                else if (n == 23)
                                {
                                    matrix[actionPoints[n].coords.X - 1, actionPoints[n].coords.Y] = 0;
                                    actionPoints[n].id_inp1 = -1;
                                    actionPoints[n].current1 = 0;
                                    roomLocks[1].GetComponent<SpriteRenderer>().enabled = false;
                                    roomLocks[1].GetComponent<Canvas>().enabled = false;

                                    roomLocks[2].GetComponent<Canvas>().enabled = true;

                                    tables[1].GetComponent<Canvas>().enabled = true;
                                }
                                else if (n == 24)
                                {
                                    matrix[actionPoints[n].coords.X - 1, actionPoints[n].coords.Y] = 4;
                                    actionPoints[n].id_inp1 = -1;
                                    actionPoints[n].current1 = 0;
                                    roomLocks[2].GetComponent<SpriteRenderer>().enabled = false;
                                    roomLocks[2].GetComponent<Canvas>().enabled = false;

                                    tables[2].GetComponent<Canvas>().enabled = true;
                                }
                            }
                        }
                        else if (n == 26)
                        {
                            if (actionPoints[n].id_inp1 == ID_in_hand && actionPoints[n].current1 <= 10)
                            {
                                actionPoints[n].current1++;
                            }
                            else if (actionPoints[n].id_inp2 == ID_in_hand && actionPoints[n].current2 <= 20)
                            {
                                actionPoints[n].current2++;
                            }
                            if (actionPoints[n].current1 >= 10 && actionPoints[n].current2 >= 20) // For unlock wire 
                            {
                                // UNLOCK ROOM
                                matrix[actionPoints[n].coords.X - 1, actionPoints[n].coords.Y] = 0;
                                actionPoints[n].id_inp1 = -1;
                                actionPoints[n].id_inp2 = -1;
                                actionPoints[n].current1 = 0;
                                actionPoints[n].current2 = 0;
                                roomLocks[4].GetComponent<SpriteRenderer>().enabled = false;
                                roomLocks[4].GetComponent<Canvas>().enabled = false;

                                roomLocks[5].GetComponent<Canvas>().enabled = true;

                                tables[4].GetComponent<Canvas>().enabled = true;
                            }
                        }
                        else if (n == 27)
                        {
                            if (actionPoints[n].id_inp1 == ID_in_hand && actionPoints[n].current1 <= 20)
                            {
                                actionPoints[n].current1++;
                            }
                            else if (actionPoints[n].id_inp2 == ID_in_hand && actionPoints[n].current2 <= 10)
                            {
                                actionPoints[n].current2++;
                            }
                            else if (actionPoints[n].id_inp3 == ID_in_hand && actionPoints[n].current3 <= 30)
                            {
                                actionPoints[n].current3++;
                            } 
                            if (actionPoints[n].current1 >= 20 && actionPoints[n].current2 >= 10 && actionPoints[n].current3 >= 30) // Microchip room
                            {
                                // UNLOCK ROOM
                                matrix[actionPoints[n].coords.X - 1, actionPoints[n].coords.Y] = 1;
                                actionPoints[n].id_inp1 = -1;
                                actionPoints[n].id_inp2 = -1;
                                actionPoints[n].id_inp3 = -1;
                                actionPoints[n].current1 = 0;
                                actionPoints[n].current2 = 0;
                                actionPoints[n].current3 = 0;
                                roomLocks[5].GetComponent<SpriteRenderer>().enabled = false;
                                roomLocks[5].GetComponent<Canvas>().enabled = false;

                                tables[5].GetComponent<Canvas>().enabled = true;
                            }
                        }
                    }
                    //Debug.Log(actionPoints[n].name + " " + actionPoints[n].current);
                    //Debug.Log(actionPoints[n+1].name + " " + actionPoints[n+1].current);
                    GetComponent<Variables>().declarations.GetDeclaration("Resource_ID").value = 0;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                    transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }
    public static string[] Images =
    {
        "", "Iron_ore", "Iron_ingot", "Iron_plate", "Iron_pipe", "Copper_ore", "Copper_ingot", "Copper_wire", "Microchip"
    };



    void FixedUpdate()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            Move();
        }
        if (Keyboard.current.aKey.isPressed)
        {
            Turn("Повернуть налево");
        }
        if (Keyboard.current.dKey.isPressed)
        {
            Turn("Повернуть направо");
        }
        if (Keyboard.current.spaceKey.isPressed)
        {
            Interact();
        }

        if (Robotics.Count == 0 && (bool)GetComponent<Variables>().declarations.GetDeclaration("Loaded").value)
            if (((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Trim() != "")
            {
                bool oneTime = (bool)GetComponent<Variables>().declarations.GetDeclaration("oneTime").value;
                Robotics = ((string)GetComponent<Variables>().declarations.GetDeclaration("Robotics").value).Split(';').ToList<string>();
                if (oneTime) GetComponent<Variables>().declarations.GetDeclaration("Loaded").value = false;
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