using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyAnimationsController))]
public class EnemyState : BaseBehaviour3D
{
    #region Props - Agent

    [Header("Agent Info")]
    [ReadOnlyProperty][SerializeField] private Vector3 _agentDestination;

    [ReadOnlyProperty][SerializeField] private float _agentRemainingDistance;
    [ReadOnlyProperty][SerializeField] private bool _agentHasPath;
    [ReadOnlyProperty][SerializeField] private bool _agentIsStopped;

    [ReadOnlyProperty][SerializeField] private Vector3 _agentVelocity;
    [ReadOnlyProperty][SerializeField] private Vector3 _agentVelocityNormalized;

    [Header("Agent Path Info")]
    [ReadOnlyProperty][SerializeField] private bool _agentPathPending;

    [ReadOnlyProperty][SerializeField] private bool _agentPathIsStale;
    [ReadOnlyProperty][SerializeField] private NavMeshPathStatus _agentPathStatus;
    [ReadOnlyProperty][SerializeField] private Vector3 _agentPathEndPosition;

    #endregion Props - Agent

    #region Props - Animation

    [ReadOnlyProperty][SerializeField] private bool _isAttacking;
    [ReadOnlyProperty][SerializeField] private bool _isDying;
    [ReadOnlyProperty][SerializeField] private bool _isMoving;
    [ReadOnlyProperty][SerializeField] private bool _isInAttackCooldown;

    #endregion Props - Animation

    #region Props - Distance from player

    [Header("Distance from player")]
    [ReadOnlyProperty][SerializeField] float _distanceFromPlayer;
    [ReadOnlyProperty][SerializeField] private bool _isOutOfReach;

    #endregion Props - Others

    #region Public Properties

    public bool IsStopped => _agentIsStopped;

    public bool IsTargetOutOfReach
    {
        get
        {
            return _agentRemainingDistance >= Agent.stoppingDistance;
            var currentPosition = transform.position;
            var playerPosition = Blackboards.Instance.PlayerBlackboard.PlayerPosition;
            _distanceFromPlayer = currentPosition.Distance(playerPosition);
            _isOutOfReach = _distanceFromPlayer >= Agent.stoppingDistance;
            return _isOutOfReach;
        }
    }

    public Vector3 PathEndPosition => _agentPathEndPosition;

    #endregion Public Properties

    #region Dependencies

    private NavMeshAgent _agent;

    private NavMeshAgent Agent => GetInitialisedComponent(ref _agent);

    private EnemyAnimationsController _enemyAnimationsController;
    private EnemyAnimationsController EnemyAnimationsController => GetInitialisedComponent(ref _enemyAnimationsController);

    #endregion Dependencies

    public void PollStateInfo()
    {
        var currentPosition = transform.position;
        var playerPosition = Blackboards.Instance.PlayerBlackboard.PlayerPosition;
        _distanceFromPlayer = currentPosition.Distance(playerPosition);

        // Agent info
        _agentDestination = Agent.destination;
        _agentRemainingDistance = Agent.remainingDistance;
        _agentHasPath = Agent.hasPath;
        _agentIsStopped = Agent.isStopped || !_agentHasPath || Agent.velocity == Vector3.zero;
        _agentVelocity = Agent.velocity;
        _agentVelocityNormalized = Agent.velocity.normalized;

        // Agent Path info
        _agentPathPending = Agent.pathPending;
        _agentPathIsStale = Agent.isPathStale;
        _agentPathStatus = Agent.pathStatus;
        _agentPathEndPosition = Agent.pathEndPosition;

        // Animation info
        _isAttacking = EnemyAnimationsController.IsAttacking;
        _isDying = EnemyAnimationsController.IsDying;
        _isMoving = EnemyAnimationsController.IsMoving;
        _isInAttackCooldown = EnemyAnimationsController.IsInAttackCooldown;
    }
}