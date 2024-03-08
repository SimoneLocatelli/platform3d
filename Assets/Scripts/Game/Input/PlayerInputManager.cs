using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerInputManager : BaseBehaviour
{
    [Header("Movement Keys")]
    public KeyCode Up = KeyCode.W;

    public KeyCode Down = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;

    [Header("Actions")]
    public KeyCode Jump = KeyCode.Space;

    public bool UpPressed { get; set; }
    public bool DownPressed { get; set; }
    public bool LeftPressed { get; set; }
    public bool RightPressed { get; set; }

    public bool JumpPressed { get; set; }
    public bool JumpPressedDown { get; set; }

    [Header("Info")]
    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 _movementVector = new Vector3();

    public Vector3 MovementVector => _movementVector;

    public void Update()
    {
        UpPressed = Input.GetKey(Up);
        DownPressed = Input.GetKey(Down);
        LeftPressed = Input.GetKey(Left);
        RightPressed = Input.GetKey(Right);
        JumpPressed = Input.GetKey(Jump);
        JumpPressedDown = Input.GetKeyDown(Jump);

        UpdateMovementVector();
    }

    private void UpdateMovementVector()
    {
        _movementVector.x = RightPressed ? 1 :
                           LeftPressed ? -1
                           : 0;

        _movementVector.z = UpPressed ? 1 :
                            DownPressed ? -1
                            : 0;
    }
}