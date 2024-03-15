using System.Diagnostics;
using System.Runtime.CompilerServices;

public class PlayerAnimationEvents : BaseBehaviour
{
    #region Events

    // Jump Animation Ready

    public delegate void OnJumpAnimationReadyHandler();

    private OnJumpAnimationReadyHandler onJumpAnimationReady;

    public event OnJumpAnimationReadyHandler OnJumpAnimationReady
    {
        add { onJumpAnimationReady += value; }
        remove { onJumpAnimationReady -= value; }
    }

    // Landing Animation Ended

    public delegate void OnLandingAnimationEndedHandler();

    private OnLandingAnimationEndedHandler onLandingAnimationEnded;

    public event OnLandingAnimationEndedHandler OnLandingAnimationEnded

    {
        add { onLandingAnimationEnded += value; }
        remove { onLandingAnimationEnded -= value; }
    }

    // Attack Animation Ended

    public delegate void OnAttackAnimationEndedHandler();

    private OnAttackAnimationEndedHandler onAttackAnimationEnded;

    public event OnAttackAnimationEndedHandler OnAttackAnimationEnded

    {
        add { onAttackAnimationEnded += value; }
        remove { onAttackAnimationEnded -= value; }
    }

    // Attack Animation Swoosh
    public delegate void OnAttackAnimationSwooshHandler();

    private OnAttackAnimationSwooshHandler onAttackAnimationSwoosh;

    public event OnAttackAnimationSwooshHandler OnAttackAnimationSwoosh

    {
        add { onAttackAnimationSwoosh += value; }
        remove { onAttackAnimationSwoosh -= value; }
    }

    #endregion Events

    #region Consts

    private const string Const_IsAttacking = "IsAttacking";

    #endregion Consts

    #region Animation Event Handlers

    private void JumpAnimationTriggerCalled()

        => onJumpAnimationReady?.Invoke();

    private void LandingAnimationEndTriggerCalled()
        => onLandingAnimationEnded?.Invoke();

    private void AttackAnimationEndTriggerCalled()
        => onAttackAnimationEnded?.Invoke();

    private void AttackAnimationSwooshCalled()
        => onAttackAnimationSwoosh?.Invoke();

    #endregion Animation Event Handlers

    #region Methods

    public void ToggleAttackAnimation(bool isActive)
        => SetAnimationParameter(Const_IsAttacking, isActive);

    private void SetAnimationParameter(string parameter, bool value)
    {
        DebugLogAnimation(parameter, value);
        Animator.SetBool(parameter, value);
    }

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough, Conditional("DEBUG")]
    private void DebugLogAnimation(string animationParamName, object animationParamValue)
        => DebugLog($"Updating animation param - {animationParamName} = [{animationParamValue}])");

    #endregion Methods
}