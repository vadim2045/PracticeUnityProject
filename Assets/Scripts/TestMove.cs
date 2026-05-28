using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMove : MonoBehaviour
{
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
    };
    private float moveDistance = 27.2f;
    Point startPoint = new Point(1, 1);
    enum Rotation
    {
        Down, Left, Up, Right
    };
    Rotation rotation = Rotation.Down;
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






    void FixedUpdate()
    {
        if(Keyboard.current.wKey.isPressed)
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
            }
        }
        if (Keyboard.current.aKey.isPressed)
        {
            rotation = (Rotation)(((int)rotation - 1 + 4) % 4);
            transform.Rotate(0, 0, 90);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            rotation = (Rotation)(((int)rotation + 1 + 4) % 4);
            transform.Rotate(0, 0, -90);
        }
    }
}
