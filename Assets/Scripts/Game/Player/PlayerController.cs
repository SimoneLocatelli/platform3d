using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(PlayerInputManager))]
public partial class PlayerController : BaseBehaviour3D
{
    public bool useRb;
    public Vector3 RbVelocity;
    public int Damage = 70;

    #region Attack Fields

    [SerializeField] private AttackCollider attackCollider;

    #endregion Attack Fields

    #region Physics Fields

    [Header("Movement")]
    [Range(1, 20)]
    [SerializeField]
    private float speed = 5;

    [Range(1, 10)]
    [SerializeField] private float runSpeed = 5;

    [Range(0.01f, 10f)]
    [SerializeField] private float turnSmoothTime = 0.2f;

    [ReadOnlyProperty]
    [SerializeField] private float turnSmoothVelocity;

    [Range(0.01f, 10f)]
    [SerializeField] private float speedSmoothTime = 0.1f;

    [ReadOnlyProperty]
    [SerializeField] private float speedSmoothVelocity;

    [ReadOnlyProperty]
    [SerializeField] private float timeSinceStartedRunning = 0;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20;

    [Range(1, 5)]
    [SerializeField] private float jumpForwardMultiplier = 1.5f;

    [Header("Physics")]
    [Range(0, 20)]
    [SerializeField] private float gravity = 9.81f;

    [Range(0, 20)]
    [SerializeField] private float gravityScale = 5;

    [Header("Animations")]
    [Range(0.3f, 5)]
    [SerializeField] private float runAnimationAccelleration = 2f;

    [ReadOnlyProperty]
    [SerializeField] private float currentSpeed;

    [Header("Stamina")]
    [Range(10, 300)]
    [SerializeField] private float staminaMax = 100;

    [ReadOnlyProperty]
    [SerializeField] private float currentStamina;

    [Range(0.1f, 100)]
    [SerializeField] private float staminaDepletionSpeed = 1;

    [Range(0.1f, 100)]
    [SerializeField] private float staminaRecoverySpeed = 1;

    [Range(0.5f, 5f)]
    [SerializeField] private float timeToStartStaminaRecovery = 0;

    [Range(0.5f, 10)]
    [SerializeField] private float runMinStaminaThreshold = 2;

    public float StaminaPercentage => currentStamina / staminaMax;

    #endregion Physics Fields

    #region Ground Check Fields

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;

    [Range(0.1f, 4)]
    [SerializeField] private float groundCheckRadius = 0.5f;

    [SerializeField] private LayerMask groundLayer;

    #endregion Ground Check Fields

    #region Animations

    [Header("Animations")]
    [SerializeField]
    private Animator _modelAnimator;

    [SerializeField]
    private Transform model;

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
    private bool isWalking;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isAttacking;

    [SerializeField]
    [ReadOnlyProperty]
    private bool attackEnded = false;

    [SerializeField]
    [ReadOnlyProperty]
    private bool isRunning;

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

    private PlayerInputManager PlayerInputManager => GetInitialisedComponent(ref _playerInputManager);

    private PlayerAnimationEvents _playerAnimationEvents;

    private PlayerAnimationEvents PlayerAnimationEvents
    {
        get
        {
            _playerAnimationEvents ??= GetComponentInChildren<PlayerAnimationEvents>();
            Assert.IsNotNull(_playerAnimationEvents);
            return _playerAnimationEvents;
        }
    }

    public Vector3 ModelOrientation => model.localEulerAngles;

    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 _modelOrientation;

    [SerializeField]
    [ReadOnlyProperty]
    private Vector3 _modelOrientationNormalised;

    private new Transform camera;

    #endregion Dependencies

    #region Life Cycle

    private void Start()
    {
        Assert.IsNotNull(model);
        Assert.IsNotNull(attackCollider);

        PlayerAnimationEvents.OnJumpAnimationReady += OnJumpAnimationReady;
        PlayerAnimationEvents.OnLandingAnimationEnded += OnLandingAnimationEnded;
        PlayerAnimationEvents.OnAttackAnimationEnded += OnAttackAnimationEnded;
        PlayerAnimationEvents.OnAttackAnimationSwoosh += OnAttackAnimationSwoosh;
        PlayerAnimationEvents.OnRunAnimationFootstep += OnRunAnimationFootstep;

        attackCollider.OnDamageApplied += OnDamageApplied;
        currentStamina = staminaMax;

        InitCamera();
    }

    private void Update()
    {
        Assert.IsNotNull(model);
        _modelOrientation = ModelOrientation;
        _modelOrientationNormalised = _modelOrientation.normalized;
        direction = GetMovementVector();
        DebugLog("direction: " + direction);
        PerformAttack();
        PerformMovement();
    }

    private void Reset()
        => InitCamera();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    #endregion Life Cycle

    #region Methods

    private void PerformAttack()
    {
        // A previous attack (animation) has ended
        if (attackEnded)
        {
            DebugLog($"{nameof(PerformAttack)} - Handling attack ended");
            // Ensure animation is not playing
            PlayerAnimationEvents.ToggleAttackAnimation(false);

            //
            attackCollider.ToggleCollider(false);

            // Let's make sure the flags are disabled
            isAttacking = false;
            attackEnded = false;
        }

        // Check if transition from non attack to attack
        if (!isAttacking)
        {
            // Player isn't attacking, if attack is not pressed, then nothing else to do for now
            if (!PlayerInputManager.AttackPressedDown)
                return;

            DebugLog($"{nameof(PerformAttack)} - Handling attack pressed");

            // Player has pressed attack
            isAttacking = true;

            // Rotate Player
            RotateTowardsCameraDirection();

            // Start Attack Animation
            PlayerAnimationEvents.ToggleAttackAnimation(true);
        }
        // Since we're here, attack is already in progress
        else
        {
            // DebugLog($"{nameof(PerformAttack)} - Handling attack in progress");

            // Rotate Player
            RotateTowardsCameraDirection();
        }
    }

    private void PerformMovement()
    {
        HandleJumpingAndFalling();

        isWalking = !isJumping && direction != Vector3.zero;

        // Player can't start running if does not have enough stamina
        UpdateRunningState();

        Vector3 directionWithSpeed;
        var canMove = direction != Vector3.zero && !isPlayingPreJumpAnimation && !isLanding;
        if (canMove)
        {
            // Rotate character towards the forward direction relative to the movement vector
            float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

            // Calculate movement with velocity
            currentSpeed = CalculateTargetSpeed();
            directionWithSpeed = (transform.forward * currentSpeed).Update(y: Rb.velocity.y);
        }
        else
        {
            directionWithSpeed = Vector3.zero;
        }

        var shouldUpdateVelocity = !isJumping && !isPlayingPreJumpAnimation && !isFalling;

        if (isPlayingPreJumpAnimation)
            Rb.velocity = Vector3.zero;
        if (shouldUpdateVelocity)
            Rb.velocity = directionWithSpeed;

        if (!isWalking && !isPlayingPreJumpAnimation)
        {
            ResetSpeed();
        }

        // Stamina update

        UpdateStamina();

        // Variables update
        var runAnimationSpeed = isRunning ? 1.25f : 1;
        LastMovementVector = direction;
        RbVelocity = Rb.velocity;

        // Animation update

        Animator.SetBool("IsJumping", isJumping);
        Animator.SetBool("IsWalking", isWalking);
        Animator.SetBool("OnGround", isGrounded);
        Animator.SetBool("IsLanding", isLanding);
        Animator.SetBool("IsFalling", isFalling);
        Animator.SetBool("IsRunning", isRunning);
        Animator.SetFloat("RunAnimSpeed", runAnimationSpeed);
    }

    private void ResetSpeed()
    {
        currentSpeed = turnSmoothVelocity = speedSmoothVelocity = 0;
    }

    private float CalculateTargetSpeed()
    {
        float targetSpeed = speed + (isRunning ? runSpeed : 0);
        return Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
    }

    private void HandleJumpingAndFalling()
    {
        if (!isJumping)
        {
            isGrounded = isGrounded || IsGrounded();
            if (isGrounded && !isAttacking)
            {
                isJumping = PlayerInputManager.JumpPressedDown;
                isPlayingPreJumpAnimation = isJumping;
                DebugLog($"Started jump", condition: isJumping);
            }

            return;
        }

        if (isPlayingPreJumpAnimation)
            return;

        if (isFalling)
        {
            if (IsGrounded())
            {
                DebugLog($"Started landing");
                AudioManager.Play("Jump Landed");
                isFalling = false;
                isJumping = false;
                isGrounded = true;
                isLanding = true;
            }
            //y = Rb.velocity.y;
        }
        else
        {
            isGrounded = false;
            if (Rb.velocity.y < 0)
            {
                DebugLog($"Started falling");

                velocity = Math.Max(velocity, 0);
                Rb.AddForce(transform.up * -4, ForceMode.Impulse);

                isFalling = true;
            }
            else if (Rb.velocity.y > 0)
            {
                DebugLog("Rb.velocity - Before: " + Rb.velocity);
                var jumpVelocity = Rb.velocity.y;
                jumpVelocity = jumpVelocity - (gravity * gravityScale * Time.deltaTime);
                Rb.velocity = Rb.velocity.Update(y: jumpVelocity);
                DebugLog("Rb.velocity -  After: " + Rb.velocity);
            }

            //y = Rb.velocity.y;
            //y = velocity;
        }
    }

    private Vector3 GetMovementVector()
    {
        if (isAttacking)
            return Vector3.zero;

        return PlayerInputManager.MovementVector;
    }

    private void UpdateRunningState()
    {
        if (isRunning)
            // Player is already running so we need to check if they continue to do so
            isRunning = isWalking && PlayerInputManager.RunPressed && currentStamina > 0;
        else
            isRunning = isWalking && PlayerInputManager.RunPressedDown && currentStamina >= runMinStaminaThreshold;

        if (isRunning)
            timeSinceStartedRunning = 0;
    }

    private void UpdateStamina()
    {
        float staminaFactor;

        if (isJumping || isAttacking)
            // No change to stamina when jumping or attacking
            staminaFactor = 0;
        else if (isRunning)
            // Full depletion speed when running
            staminaFactor = -staminaDepletionSpeed;
        else if (isWalking)
            // Half depletion speed when walking
            staminaFactor = staminaRecoverySpeed / 2;
        else
            // Full recovery speed if player is not performing actions
            staminaFactor = staminaRecoverySpeed;

        var isPlayerRecovering = staminaFactor > 0;

        if (isPlayerRecovering)
        {
            // Full stamina - so no recovery
            if (currentStamina == staminaMax)
                staminaFactor = 0;

            // If not enough time has passed since player stopped running, then no recovery at all
            if (timeSinceStartedRunning < timeToStartStaminaRecovery)
            {
                timeSinceStartedRunning += Time.deltaTime;
                staminaFactor = 0;
            }
        }

        // No recovery - no update needed
        if (staminaFactor == 0)
            return;

        currentStamina += staminaFactor * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, staminaMax);
        currentStamina = FloatRounding.Round(currentStamina, 2);
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
        => Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

    /// <summary>
    /// This rotates the player object so that it looks in the same direction as the camera.
    /// Useful when we want to make the player look 'straight/forward' relative to the camera.
    /// </summary>
    private void RotateTowardsCameraDirection()
    {
        float targetRotation = Mathf.Atan2(y: 0, x: 0) * Mathf.Rad2Deg + camera.eulerAngles.y;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, 0.01f);
    }

    private void InitCamera()
    {
        if (camera == null)
            camera = Blackboards.Instance.CameraBlackboard.CameraTransform;
    }

    #endregion Methods

    #region Animation Triggers

    private void OnAttackAnimationEnded()
    {
        DebugLogMethodEntry();
        if (PlayerInputManager.AttackPressedDown)
            return;
        attackEnded = true;
        isAttacking = false;
    }

    private void OnAttackAnimationSwoosh()
    {
        AudioManager.Play("Sword_Woosh");
        attackCollider.ToggleCollider(true);
    }

    private void OnJumpAnimationReady()
    {
        DebugLog($"{nameof(OnJumpAnimationReady)}- Called");
        isPlayingPreJumpAnimation = false;
        velocity = jumpForce;
        isJumping = true;

        DebugLog($"Jump - Added force", condition: isJumping);
        Vector3 dir = Vector3.zero;
        Vector3 jumpDir;

        if (LastMovementVector != Vector3.zero)
        {
            dir = transform.forward * currentSpeed * jumpForwardMultiplier;
            jumpDir = dir.Update(y: transform.up.y * (jumpForce));
        }
        else
        {
            jumpDir = dir.Update(y: transform.up.y * jumpForce);
        }
        Debug.Log(jumpDir);

        Rb.AddForce(jumpDir, ForceMode.Impulse);
        // Rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        //transform.Translate(LastMovementVector * Time.deltaTime);
    }

    private void OnLandingAnimationEnded()
    {
        isLanding = false;
        isJumping = false;
    }

    private void OnRunAnimationFootstep()
    {
        AudioManager.Play("Footstep");
    }

    #endregion Animation Triggers

    #region Attack Collider Event Handlers

    private void OnDamageApplied(GameObject objectHit)
    {
        DebugLogMethodEntry();
        Assert.IsNotNull(objectHit);

        if (objectHit.HasAllTags(Tags.Collections.Object_Wood))
            AudioManager.Play("Sword_Hit_Wood");
        else if (objectHit.HasAllTags(Tags.Collections.NPC_Skeleton))
            AudioManager.Play("Sword_Bone_Hit");
    }

    #endregion Attack Collider Event Handlers
}