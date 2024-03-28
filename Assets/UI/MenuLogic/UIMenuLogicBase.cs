using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public abstract class UIMenuLogicBase : BaseBehaviourLight
{
    private OnMenuExitHandler _onMenuExit;
    private AudioClip ButtonPressedAudioClip;

    protected UIDocument UIDocument { get; private set; }

    public delegate void OnMenuExitHandler();
    public event OnMenuExitHandler OnMenuExit
    {
        add { _onMenuExit += value; }
        remove { _onMenuExit += value; }
    }

    protected void ExitMenu()
    {
        this.Destroy();
        _onMenuExit?.Invoke();
    }

    protected virtual void OnEnable()
    {
        UIDocument = GetComponent<UIDocument>();
        Assert.IsNotNull(UIDocument);
    }
}