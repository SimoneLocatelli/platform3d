using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(EnemyAnimationsController))]
[RequireComponent(typeof(LifeSystem))]
[RequireComponent(typeof(EnemyState))]
public class EnemyController : BaseBehaviour3D
{
    #region Props - Path Finding

    [Header("Path Finding")]
    [SerializeField] private bool enableNavMeshAgent;

    [Range(6f, 20f)]
    [SerializeField] private float maxDistanceFromPlayer;

    [Header("Combat")]
    [Range(0.1f, 2f)]
    [SerializeField] private float attackCoolDown;

    [ReadOnlyProperty][SerializeField] private float remainingAttackCooldown;

    #endregion Props - Path Finding

    #region Props - Can See Player

    [Header("Can See Player")]
    [Range(10, 100)]
    [SerializeField] private int maxPlayerVisibilityDistance = 30;

    [SerializeField] private bool drawCanSeePlayerRays;

    #endregion Props - Can See Player

    #region Dependencies

    private NavMeshAgent _agent;

    public NavMeshAgent Agent => GetInitialisedComponent(ref _agent);

    private LifeSystem _lifeSystem;

    private LifeSystem LifeSystem => GetInitialisedComponent(ref _lifeSystem);

    private EnemyAnimationsController _enemyAnimationsController;

    private EnemyAnimationsController EnemyAnimationsController => GetInitialisedComponent(ref _enemyAnimationsController);

    private Transform PlayerTransform => Blackboards.Instance.PlayerBlackboard.PlayerTransform;

    private Vector3 PlayerPosition => PlayerTransform.position;

    private EnemyState _enemyState;

    private EnemyState EnemyState => GetInitialisedComponent(ref _enemyState);

    private AttackCollider attackCollider;

    #endregion Dependencies

    #region Properties

    public bool CanMove => enableNavMeshAgent && !IsAttacking;

    #endregion Properties

    #region Properties - Animation Parameters

    public bool IsAttacking
    {
        get => EnemyAnimationsController.IsAttacking;
        set => EnemyAnimationsController.IsAttacking = value;
    }

    public bool IsDying
    {
        get => EnemyAnimationsController.IsDying;
        set => EnemyAnimationsController.IsDying = value;
    }

    public bool IsInAttackCooldown
    {
        get => EnemyAnimationsController.IsInAttackCooldown;
        set => EnemyAnimationsController.IsInAttackCooldown = value;
    }

    public bool IsMoving
    {
        get => EnemyAnimationsController.IsMoving;
        set => EnemyAnimationsController.IsMoving = value;
    }

    #endregion Properties - Animation Parameters

    private void Start()
    {
        attackCollider = gameObject.GetComponentInChildren<AttackCollider>(includeInactive: false);
        LifeSystem.OnDeath += OnDeath;
        UpdateAgentTarget();
    }

    private void OnDeath(LifeSystem lifeSystem)
    {
        IsDying = true;
        IsMoving = false;
        IsAttacking = false;
        ClearAgentTarget();
    }

    private void Update()
    {
        if (LifeSystem.IsDead)
            return;

        EnemyState.PollStateInfo();

        UpdateAgentTarget();

        var canSeePlayer = CanSeePlayer();

        if (!canSeePlayer && Agent.remainingDistance > maxDistanceFromPlayer)
        {
            Agent.isStopped = true;
            IsMoving = false;
            IsAttacking = false;
            IsInAttackCooldown = false;
            return;
        }

        // LookAtTarget();

        // Enable/disable moving animation
        IsMoving = !EnemyState.IsStopped;

        PerformAttack();
    }

    private bool CanSeePlayer()
    {
        var playerPosition = Blackboards.Instance.PlayerBlackboard.PlayerPosition;
        var layerMask = Blackboards.Instance.PlayerBlackboard.PlayerLayerMask;
        var currentPosition = transform.position;
        var direction = transform.TransformDirection(Vector3.forward);
        direction = currentPosition.GetDirectionNormalised(PlayerPosition);
        var maxDistance = this.maxPlayerVisibilityDistance;

        // Does the ray intersect any objects excluding the player layer
        var playerHit = Physics.Raycast(transform.position, direction, out RaycastHit hit, maxDistance, layerMask);
        if (drawCanSeePlayerRays)
        {
            var distance = playerHit ? hit.distance : 1000;
            Debug.DrawRay(currentPosition, direction * maxDistance, Color.yellow);
        }
        DebugLog("CanSeePlayer: [" + playerHit + "]");
        return playerHit;
    }

    private void PerformAttack()
    {
        Assert.IsNotNull(attackCollider, "AttackCollider is not assigned.");

        // If attack is in progress, we ensure the Agent does not try to follow the player
        // who might have moved to avoid the attack
        if (IsAttacking)
        {
            Agent.isStopped = true;
            return;
        }

        Agent.isStopped = false;

        // Attack Collider is always off when not attacking
        attackCollider.ToggleCollider(false);

        // If path is being recalculated, make no further changes
        if (Agent.pathPending)
            return;

        // If it's moving or target is too far to be attacked,
        // then perform reset and do not attempt attack
        if (!EnemyState.IsStopped || EnemyState.IsTargetOutOfReach)
        {
            // Reset attack state
            IsInAttackCooldown = false;
            remainingAttackCooldown = 0;
            return;
        }

        ClearAgentTarget();

        // Is waiting for attack cooldown?
        if (remainingAttackCooldown > 0)
        {
            DebugLog("Attack Cooldown");
            IsInAttackCooldown = true;
            remainingAttackCooldown -= Time.deltaTime;
            remainingAttackCooldown = Mathf.Max(remainingAttackCooldown, 0);
            return;
        }

        // We can start the attack!
        DebugLog("Melee attack");
        IsInAttackCooldown = false;
        IsAttacking = true;
        remainingAttackCooldown = attackCoolDown;
    }

    private void LookAtTarget()
    {
        if (IsAttacking)
            return;
        else if (enableNavMeshAgent && EnemyState.IsStopped)
            transform.LookAt(EnemyState.PathEndPosition);
    }

    private void UpdateAgentTarget()
    {
        Agent.destination = PlayerPosition;
    }

    private void ClearAgentTarget()
        => Agent.destination = transform.position;

    #region Animation Triggers

    private void OnMeleeAttackSwoosh()
    {

        if (attackCollider != null)
            attackCollider.ToggleCollider(true);
        else
            Debug.LogWarning("Attack collider is null");
    }

    private void OnMeleeAttackSwooshEnded()
    {
        if (attackCollider != null)
            attackCollider.ToggleCollider(false);
        else
            Debug.LogWarning("Attack collider is null");
    }

    #endregion Animation Triggers
}