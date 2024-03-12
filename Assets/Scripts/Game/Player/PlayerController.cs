using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PlayerInputManager))]
public partial class PlayerController : BaseBehaviour
{
    #region Physics Fields

    [Header("Movement")]
    [Range(1, 5)]
    [SerializeField]
    private float speed = 5;

    [Range(1, 10)]
    [SerializeField]
    private float runSpeed = 5;

    [SerializeField]
    private float jumpForce = 20;

    [Range(0, 20)]
    [SerializeField]
    private float gravity = 9.81f;

    [Range(0, 20)]
    [SerializeField]
    private float gravityScale = 5;

    [SerializeField]
    [Range(0.01f, 10f)]
    private float turnSmoothTime = 0.2f;

    [SerializeField]
    [ReadOnlyProperty]
    private float turnSmoothVelocity;

    [SerializeField]
    [Range(0.01f, 10f)]
    private float speedSmoothTime = 0.1f;

    [SerializeField]
    [ReadOnlyProperty]
    private float speedSmoothVelocity;

    [SerializeField]
    [Range(0.3f, 5)]
    private float runAnimationAccelleration = 2f;

    [SerializeField]
    [ReadOnlyProperty]
    private float currentSpeed;

    [SerializeField]
    [Range(10, 300)]
    private float staminaMax = 100;

    [SerializeField]
    [Range(0.1f, 100)]
    private float staminaDepletionSpeed = 1;

    [SerializeField]
    [ReadOnlyProperty]
    private float currentStamina;

    public float StaminaPercentage => currentStamina / staminaMax;

    #endregion Physics Fields

    #region Ground Check Fields

    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayer;

    #endregion Ground Check Fields

    #region Animations

    [Header("Animations")]
    [SerializeField]
    private Animator _modelAnimator;

    [SerializeField]
    private Transform Model;

    #endregion Animations

    #region Info Fields

    [Header("Debug")]
    [ReadOnlyProperty]
    [SerializeField]
    private Vector3 direction;

    [SerializeField]
    [ReadOnlyProperty]
    private float velocity;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isGrounded;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isLanding;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isJumping;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isPlayingPreJumpAnimation;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isFalling;

    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 LastMovementVector;

    #endregion Info Fields

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

        currentStamina = staminaMax;
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
        var inputVector = PlayerInputManager.MovementVector;
        var movementJumpFactor = (isJumping ? 5 : 1);
        var x = inputVector.x * speed;
        var z = inputVector.z * speed;
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

        var movementVector = new Vector3(x, 0, z);
        var isWalking = !isJumping && currentMovement != Vector3.zero;
        var isRunning = isWalking && PlayerInputManager.RunPressed;

        float targetSpeed = speed + (isRunning ? runSpeed : 0); //* movementVector.magnitude;
        if (movementVector != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(movementVector.x, movementVector.z) * Mathf.Rad2Deg + Camera.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        var dir = inputVector == Vector3.zero ? Vector3.zero : transform.forward * currentSpeed;
        dir.y = y;
        transform.Translate(dir * Time.deltaTime, Space.World);

        var runAnimationSpeed = Animator.GetFloat("RunAnimSpeed");
        if (!isWalking && !isJumping)
        {
            currentSpeed = turnSmoothVelocity = speedSmoothVelocity = 0;
        }

        if (!isWalking)
            runAnimationSpeed = 1;
        else
        {
            if (LastMovementVector.x == 0 && LastMovementVector.z == 0) // no walking movement
            {
                runAnimationSpeed = 0;
            }
            else if (runAnimationSpeed < 1)
            {
                runAnimationSpeed += runAnimationAccelleration * Time.deltaTime;
                runAnimationSpeed = Mathf.Min(runAnimationSpeed, 1);
            }
        }

        // Stamina update

        if (isWalking)
        {
            if (isRunning)
                currentStamina -= staminaDepletionSpeed * Time.deltaTime;
            else
                currentStamina += staminaDepletionSpeed / 2 * Time.deltaTime;
        }
        else if (!isJumping)
        {
            currentStamina += staminaDepletionSpeed * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, staminaMax);

        // Animation update
        Animator.SetBool("IsJumping", isJumping);
        Animator.SetBool("IsWalking", isWalking);
        Animator.SetBool("OnGround", isGrounded);
        Animator.SetBool("IsLanding", isLanding);
        Animator.SetBool("IsFalling", isFalling);
        Animator.SetBool("IsRunning", isRunning);
        Animator.SetFloat("RunAnimSpeed", runAnimationSpeed);

        LastMovementVector = movementVector;
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