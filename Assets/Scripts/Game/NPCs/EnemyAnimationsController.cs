using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationsController : BaseBehaviour3D

{
    #region Animation Parameters


    public bool IsDying
    {
        get => Animator.GetBool("IsDying");
        set => Animator.SetBool("IsDying", value);
    }

    public bool IsMoving
    {
        get => Animator.GetBool("IsMoving");
        set => Animator.SetBool("IsMoving", value);
    }

    public bool IsAttacking
    {
        get => Animator.GetBool("IsAttacking");
        set => Animator.SetBool("IsAttacking", value);
    }

    public bool IsInAttackCooldown
    {
        get => Animator.GetBool("IsInAttackCooldown");
        set => Animator.SetBool("IsInAttackCooldown", value);
    }

    #endregion Animation Parameters

    #region Events

    #endregion Events

    public delegate void OnAttackEndedHandler();

    public event OnAttackEndedHandler OnAttackEnded;

    #region Animation Event Callbacks

    public void OnAttackEndedCallback()
    {
        IsAttacking = false;
        IsInAttackCooldown = true;
        OnAttackEnded?.Invoke();
    }

    #endregion Animation Event Callbacks
}