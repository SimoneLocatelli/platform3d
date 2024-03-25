using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(int.MaxValue)]
public abstract class BaseBlackboard : BaseBehaviourLight
{
    private readonly string blackboardTypeName;

    protected BaseBlackboard()
    {
        blackboardTypeName = this.GetType().FullName;
    }

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    protected TGameObject FindObjectOfType<TGameObject>(ref TGameObject gameObject) where TGameObject : UnityEngine.Object
    {
        if (gameObject != null)
            return gameObject;

        var obj = GameObject.FindFirstObjectByType<TGameObject>(FindObjectsInactive.Exclude);

        Assert.IsNotNull(obj, $"Blackboard {blackboardTypeName} - Could not find object of type [{typeof(TGameObject).FullName}");

        return obj;
    }

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    protected TComponent GetInitialisedComponent<TComponent>(GameObject gameObject, ref TComponent component) where TComponent : Component
    {
        Assert.IsNotNull(gameObject);
        if (component != null)
            return component;

        component = gameObject.GetComponent<TComponent>();

        Assert.IsNotNull(component, $"Blackboard {blackboardTypeName} - Could not find component of type [{typeof(TComponent).FullName}");

        return component;
    }
}