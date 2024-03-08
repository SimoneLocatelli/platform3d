using System;
using System.ComponentModel;
using UnityEngine;

public class PlayerMovementState : BaseBehaviour
{

    [Header("Settings")]
    [Range(1, 30)]
    public int Speed = 5;

    [Range(20, 150)]
    public int RotationSpeed = 60;

    [Range(1, 30)]
    public int JumpSpeed = 15;

    public float JumpForce = 20;

    [ReadOnlyProperty]
    public float Gravity = -9.81f;
    
    public float GravityScale = 5;

    public float Velocity;


    [Header("Ground Check")]
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private LayerMask groundLayer;

    [Header("Debug State")]
    [ReadOnlyProperty]
    [SerializeField]
    private Vector3 direction;

    [ReadOnlyProperty]
    [SerializeField]
    private bool isGrounded;

    [ReadOnlyProperty]
    public bool IsJumping;


    public Vector3 Direction => direction;

    public bool IsGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.1f, groundLayer);
        return isGrounded;
    }

    public void UpdateDirection(float x, float y, float z)
           => direction = new Vector3(x, y, z);
}