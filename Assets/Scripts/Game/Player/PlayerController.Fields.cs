using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

// Properties Class
public partial class PlayerController : BaseBehaviour
{
    #region Physics Fields

    [Header("Movement")]
    [Range(1, 5)]
    [SerializeField]
    private float speed = 5;

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
    [ReadOnlyProperty]
    private float currentSpeed;

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
}