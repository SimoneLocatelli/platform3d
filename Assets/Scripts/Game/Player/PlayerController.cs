using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static CameraMovement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovementState))]
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerController : BaseBehaviour
{

    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 LastMovementVector;
    private bool isJumping;

    private PlayerInputManager _playerInputManager;

    private Rigidbody _rigidbody;

    private PlayerMovementState _playerMovementState;

    public Transform Model;

    [SerializeField]
    private Animator _modelAnimator;

    protected override Animator Animator => _modelAnimator;

    private PlayerInputManager PlayerInputManager

    { get { return GetInitialisedComponent(ref _playerInputManager); } }

    private PlayerMovementState PlayerMovementState
    { get { return GetInitialisedComponent(ref _playerMovementState); } }

    private Rigidbody Rigidbody
    { get { return GetInitialisedComponent(ref _rigidbody); } }

    private void Update()
    {
        Assert.IsNotNull(Model);

        UpdateMovement();
        PerformMovement();
        PerformRotation();

    }

    private void PerformMovement()
    {
        PlayerMovementState.Velocity += PlayerMovementState.Gravity * PlayerMovementState.GravityScale * Time.deltaTime;

        Animator.SetBool("IsFalling", PlayerMovementState.Velocity < 0);

        if (PlayerMovementState.IsGrounded() && PlayerMovementState.Velocity < 0)
        {
            PlayerMovementState.Velocity = 0;
            Animator.SetBool("OnGround", true);
        }
        if (PlayerInputManager.JumpPressedDown)
            PlayerMovementState.Velocity = PlayerMovementState.JumpForce;

        var x = PlayerMovementState.Direction.x * PlayerMovementState.Speed;
        var y = PlayerMovementState.Velocity;
        var z = PlayerMovementState.Direction.z * PlayerMovementState.Speed;

        LastMovementVector = new Vector3(x, y, z);
//        transform.Translate(LastMovementVector * Time.deltaTime);


        // var playerDirection = PlayerMovementState.Direction;

        //  if (playerDirection == Vector3.zero)
        return;

        //transform.position = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;

        // transform.Translate(PlayerMovementState.Direction * PlayerMovementState.Speed * Time.deltaTime);
    }

    private void PerformRotation()
    {
        if (PlayerMovementState.Direction == Vector3.zero)
            return;
        var currentRotation = Model.rotation.eulerAngles;

        float rotationY = 0;

        if (PlayerMovementState.Direction.z == 0)
        {
            if (PlayerMovementState.Direction.x > 0)
                rotationY = 90;
            else if (PlayerMovementState.Direction.x < 0)
                rotationY = -90;
        }

        else
        {
            if (PlayerMovementState.Direction.z > 0)
            {

                if (PlayerMovementState.Direction.x > 0)
                    rotationY = 45;
                else if (PlayerMovementState.Direction.x < 0)
                    rotationY = -45;
                else
                    rotationY = 0;
            }
            else
            {
                if (PlayerMovementState.Direction.x > 0)
                    rotationY = 180 - 45;
                else if (PlayerMovementState.Direction.x < 0)
                    rotationY = 180 + 45;
                else
                    rotationY = 180;

            }
        }
        var rotationOffset = 25; // necessary to make the model look straight as it is slightly rotated.
        currentRotation.y = rotationY;// + rotationOffset;

        /*

                var delta = 1f * PlayerMovementState.RotationSpeed;
                if (PlayerMovementState.Direction.x > 0)
                    delta = -delta;

                delta = Time.deltaTime * delta;
                currentRotation = currentRotation.Update(y: currentRotation.y - delta);
         */

        Model.localEulerAngles = currentRotation;
    }


    [SerializeField]
    private bool WaitForJumpKeyUp = false;
    private void UpdateMovement()
    {
        var baseSpeed = 1;

        var x = PlayerInputManager.RightPressed ? baseSpeed :
                PlayerInputManager.LeftPressed ? -baseSpeed
                : 0;

        var z = PlayerInputManager.UpPressed ? baseSpeed :
                PlayerInputManager.DownPressed ? -baseSpeed
                : 0;

        var y = 0f;

        if (WaitForJumpKeyUp)
        {


            if (!PlayerInputManager.JumpPressed)
                WaitForJumpKeyUp = false;
        }
        else if (!PlayerMovementState.IsJumping && PlayerInputManager.JumpPressedDown && PlayerMovementState.IsGrounded())
        {
            y = PlayerMovementState.JumpSpeed;
            WaitForJumpKeyUp = true;
            PlayerMovementState.IsJumping = PlayerInputManager.JumpPressed;
        }
        else if (!PlayerInputManager.JumpPressedDown && PlayerMovementState.IsGrounded())
        {
            PlayerMovementState.IsJumping = false;
        }


        PlayerMovementState.UpdateDirection(x, y, z);
        Animator.SetBool("IsJumping", PlayerMovementState.IsJumping);
        Animator.SetBool("IsWalking", !PlayerMovementState.IsJumping && PlayerMovementState.Direction != Vector3.zero);
        Animator.SetBool("OnGround", !PlayerMovementState.IsJumping);


        /*
         


        var currentVelocity = Rigidbody.velocity;
        var x = 0f;
        var y = 0f;
        var z = 0f;

        var baseSpeed = PlayerMovementState.Speed; 

        if (PlayerInputManager.LeftPressed)
            x = -baseSpeed;
        else if (PlayerInputManager.RightPressed)
            x = baseSpeed;
        else
            x = currentVelocity.x;

        if (PlayerInputManager.UpPressed)
            z = baseSpeed;
        else if (PlayerInputManager.DownPressed)
            z = -baseSpeed;
        else
            z = currentVelocity.z;

        PlayerMovementState.IsJumping = PlayerInputManager.JumpPressed;

        if (PlayerInputManager.JumpPressedDown && PlayerMovementState.IsGrounded())
            y = PlayerMovementState.JumpSpeed;
        else
            y= currentVelocity.y;

        PlayerMovementState.UpdateDirection(x, y, z);
         */
    }
}