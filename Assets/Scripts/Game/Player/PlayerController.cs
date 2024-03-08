using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using static CameraMovement;

[RequireComponent(typeof(PlayerInputManager))]
public partial class PlayerController : BaseBehaviour
{
    #region Dependencies

    private PlayerInputManager _playerInputManager;

    protected override Animator Animator => _modelAnimator;

    private PlayerInputManager PlayerInputManager
        => GetInitialisedComponent(ref _playerInputManager);

    PlayerAnimationEvents playerAnimationEvents;

    #endregion Dependencies

    #region Life Cycle

    private void Start()
    {
        var playerAnimationEvents = GetComponentInChildren<PlayerAnimationEvents>();

        Assert.IsNotNull(playerAnimationEvents);

        playerAnimationEvents.OnJumpAnimationReady += OnJumpAnimationReady;
        playerAnimationEvents.OnLandingAnimationEnded += OnLandingAnimationEnded;
    }
    private void Update()
    {
        Assert.IsNotNull(Model);

        direction = PlayerInputManager.MovementVector;
        PerformMovement();
        PerformRotation();
    }

    #endregion Life Cycle

    #region Methods

    private void PerformMovement()
    {
        var movementVector = PlayerInputManager.MovementVector;
        var movementJumpFactor = (isJumping ? 5 : 1);
        var x = movementVector.x * speed;
        var z = movementVector.z * speed;
        var y = 0f;
        var currentMovement = new Vector3(x, y, z) * movementJumpFactor;

        if (!isJumping)
        {
            isJumping = PlayerInputManager.JumpPressedDown;
            isPlayingPreJumpAnimation = isJumping;
            DebugLog($"Started jump", condition: isJumping);
        }
        else
        {
            if (!isPlayingPreJumpAnimation)
            {
                velocity += -gravity * gravityScale * Time.deltaTime;


                if (velocity <= 0)
                {
                    velocity = Math.Max(velocity, 0);
                    isFalling = true;
                }

                y = velocity;


                if (isFalling)
                {
                    if (IsGrounded())
                    {
                        isFalling = false;
                        isJumping = false;
                        isGrounded = true;
                        isLanding = true;
                    }
                }
            }

        }

        LastMovementVector = new Vector3(x, y, z);
        transform.Translate(LastMovementVector * Time.deltaTime);
        var isWalking = !isJumping && currentMovement != Vector3.zero;

        Animator.SetBool("IsJumping", isJumping);
        Animator.SetBool("IsWalking", isWalking);
        Animator.SetBool("OnGround", isGrounded);
        Animator.SetBool("IsLanding", isLanding);
        Animator.SetBool("IsFalling", isFalling);
    }

    private void PerformRotation()
    {
        if (direction == Vector3.zero)
            return;
        var currentRotation = Model.rotation.eulerAngles;

        currentRotation.y = CalculateRotation();

        Model.localEulerAngles = currentRotation;
    }

    private float CalculateRotation()
    {
        if (direction.z == 0)
        {
            if (direction.x == 0)
                return 0;

            return 90 * (direction.x > 0 ? 1 : -1);
        }

        if (direction.z > 0)
            return direction.x == 0 ? 0 :
                   direction.x > 0 ? 45 : -45;

        if (direction.x == 0)
            return 180;

        return 180 - (direction.x > 0 ? 45 : -45);
    }

    public bool IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        return isGrounded;
    }

    #endregion Methods


    #region Animation Triggers


    private void OnJumpAnimationReady()
    {
        DebugLog($"{nameof(OnJumpAnimationReady)}- Called");
        isPlayingPreJumpAnimation = false;
        velocity = jumpForce;
        isJumping = true;

        LastMovementVector.y = velocity;
        transform.Translate(LastMovementVector * Time.deltaTime);
    }


    private void OnLandingAnimationEnded()
    {
        isLanding = false;
        isJumping = false;
    }


    #endregion
}