using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    #endregion Events

    private void JumpAnimationTriggerCalled()
        => onJumpAnimationReady?.Invoke();

    private void LandingAnimationEndTriggerCalled()
        => onLandingAnimationEnded?.Invoke();

    private void AttackAnimationEndTriggerCalled()
        => onAttackAnimationEnded?.Invoke();
}