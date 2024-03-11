using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerInputManager))]
public partial class PlayerController : BaseBehaviour
{
    #region Dependencies

    private PlayerInputManager _playerInputManager;

    protected override Animator Animator => _modelAnimator;

    private PlayerInputManager PlayerInputManager
        => GetInitialisedComponent(ref _playerInputManager);

    private PlayerAnimationEvents playerAnimationEvents;

    public Vector3 ModelOrientation => Model.localEulerAngles;

    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 _modelOrientation;

    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 _modelOrientationNormalised;

    public Transform Camera;

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

        _modelOrientation = ModelOrientation;
        _modelOrientationNormalised = _modelOrientation.normalized;
        direction = PlayerInputManager.MovementVector;
        PerformMovement();
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

        LastMovementVector = new Vector3(x, 0, z);

        bool running = false;
        float runSpeed = speed;

        float targetSpeed = ((running) ? runSpeed : speed) * LastMovementVector.magnitude;
        if (LastMovementVector != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(LastMovementVector.x, LastMovementVector.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        var dir = movementVector == Vector3.zero ? Vector3.zero : transform.forward * currentSpeed;
        dir.y = y;
        transform.Translate(dir * Time.deltaTime, Space.World);

        var isWalking = !isJumping && currentMovement != Vector3.zero;

        Animator.SetBool("IsJumping", isJumping);
        Animator.SetBool("IsWalking", isWalking);
        Animator.SetBool("OnGround", isGrounded);
        Animator.SetBool("IsLanding", isLanding);
        Animator.SetBool("IsFalling", isFalling);
    }

    private float CalculateRotation(float currentRotation)
    {
        if (!PlayerInputManager.RightPressedDown && !PlayerInputManager.LeftPressedDown)
            return currentRotation;

        var sign = PlayerInputManager.RightPressedDown ? 1 : -1;

        if (direction.z == 0)
            currentRotation += 90 * sign;
        else
        {
            if (direction.z < 0)
                currentRotation += 180;

            currentRotation += 45 * sign;
        }
        return currentRotation;
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

    #endregion Animation Triggers
}