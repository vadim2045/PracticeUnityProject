using UnityEngine;
using UnityEngine.InputSystem;

public class AllowWADs : MonoBehaviour
{
    void Update()
    {
        if (RobotMovement.movement != 8)
        {
            if (RobotMovement.movement == 0 && Keyboard.current.mKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 1 && Keyboard.current.oKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 2 && Keyboard.current.vKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 3 && Keyboard.current.eKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 4 && Keyboard.current.mKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 5 && Keyboard.current.eKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 6 && Keyboard.current.nKey.isPressed) { RobotMovement.movement++; }
            else if (RobotMovement.movement == 7 && Keyboard.current.tKey.isPressed) { RobotMovement.movement++; }
        }
    }
}
