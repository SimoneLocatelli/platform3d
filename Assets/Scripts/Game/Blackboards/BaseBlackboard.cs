using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseBlackboard
{
    private readonly string blackboardTypeName;

    protected BaseBlackboard()
    {
        blackboardTypeName = this.GetType().FullName;
    }

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    protected TGameObject FindObjectOfType<TGameObject>(ref TGameObject gameObject) where TGameObject : Object
    {
        if (gameObject != null)
            return gameObject;

        var obj = Object.FindObjectOfType<TGameObject>();

        Assert.IsNotNull(obj, $"Blackboard {blackboardTypeName} - Could not find object of type [{typeof(TGameObject).FullName}");

        return obj;
    }
}