using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : BaseBehaviour3D

{
    private NavMeshAgent _agent;

    [SerializeField]
    [ReadOnlyProperty]
    private float remainingDistance;

    [Range(0.1f, 25f)]
    [SerializeField] private float speed = 16;

    [Header("Path Finding")]
    [Range(0.1f, 25f)]
    [SerializeField] private float stoppingDistance = 3;

    [SerializeField]
    private bool canFollowTarget;

    public NavMeshAgent Agent => GetInitialisedComponent(ref _agent);

    [ReadOnlyProperty]
    [SerializeField] private bool hasPath;

    [ReadOnlyProperty]
    [SerializeField] private bool isStopped;

    [ReadOnlyProperty]
    [SerializeField] private Vector3 velocity;

    [ReadOnlyProperty]
    [SerializeField] private Vector3 velocityNormalized;

    [ReadOnlyProperty]
    [SerializeField] private bool pathPending;

    [ReadOnlyProperty]
    [SerializeField] private bool isPathStale;

    [ReadOnlyProperty]
    [SerializeField] private NavMeshPathStatus pathStatus;

    [ReadOnlyProperty]
    [SerializeField] private Vector3 pathEndPosition;

    [ReadOnlyProperty]
    [SerializeField] private Vector3 rbVelocity;

    private void Update()
    {
        Agent.stoppingDistance = stoppingDistance;
        //Agent.speed = speed;
        if (canFollowTarget)
            Agent.destination = Blackboards.Instance.PlayerBlackboard.PlayerTransform.position;
        else
            Agent.destination = transform.position;
        remainingDistance = Agent.remainingDistance;
        hasPath = Agent.hasPath;
        isStopped = Agent.velocity == Vector3.zero;


        velocity = Agent.velocity;
        velocityNormalized = Agent.velocity.normalized;
        pathPending = Agent.pathPending;
        isPathStale = Agent.isPathStale;
        pathStatus = Agent.pathStatus;
        pathEndPosition = Agent.pathEndPosition;


        if (isStopped)
            transform.LookAt(pathEndPosition);
       
        Animator.SetBool("IsMoving", !isStopped);
    }
}