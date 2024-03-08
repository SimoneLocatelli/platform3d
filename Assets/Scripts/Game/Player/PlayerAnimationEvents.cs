using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerAnimationEvents : BaseBehaviour
{
    private OnJumpAnimationReadyHandler onJumpAnimationReady;

    public delegate void OnJumpAnimationReadyHandler();
    public event OnJumpAnimationReadyHandler OnJumpAnimationReady
    {
        add { onJumpAnimationReady += value; }
        remove { onJumpAnimationReady -= value; }
    }

    private OnLandingAnimationEndedHandler onLandingAnimationEnded;

    public delegate void OnLandingAnimationEndedHandler();
    public event OnLandingAnimationEndedHandler OnLandingAnimationEnded

    {
        add { onLandingAnimationEnded += value; }
        remove { onLandingAnimationEnded -= value; }
    }

    private void JumpAnimationTriggerCalled()
        => onJumpAnimationReady?.Invoke();


    private void LandingAnimationEndTriggerCalled()
        => onLandingAnimationEnded?.Invoke();
}