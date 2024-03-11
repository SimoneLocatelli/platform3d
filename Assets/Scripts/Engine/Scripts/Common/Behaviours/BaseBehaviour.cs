using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseBehaviour : MonoBehaviour
{
    #region Properties

    [Header("Logging", order = 10000)]
    public bool EnableDebugLog = false;
    private Animator _animator;
    private AudioManager _audioManager;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    public GameObject ParentGameObject
    {
        get
        {
            var parent = transform.parent;
            if (parent == null) return null;
            return parent.gameObject;
        }
    }

    public SpriteRenderer ParentSpriteRenderer { get => ParentGameObject.GetComponent<SpriteRenderer>(); }

    public GameObject TopParent { get => gameObject.GetTopParent(); }

    protected virtual Animator Animator { get => GetInitialisedComponent(ref _animator); }

    protected AudioManager AudioManager { get => GetInitialisedComponent(ref _audioManager); }

    protected Rigidbody2D Rb { get => GetInitialisedComponent(ref _rb); }

    protected Vector3 RendererCenter { get => SpriteRenderer.bounds.center; }

    protected SpriteRenderer SpriteRenderer { get => GetInitialisedComponent(ref _spriteRenderer); }

    #endregion Properties

    #region Methods

    public static TComponent GetInitialisedComponent<TComponent>(Behaviour behaviour, ref TComponent innerReference) where TComponent : Component
    {
        if (innerReference == null)
            innerReference = behaviour.GetComponent<TComponent>();

        return innerReference;
    }

    public Collider2D[] GetCollidersInRange(float radius)
        => Physics2D.OverlapCircleAll(transform.position, radius);

    public Collider2D[] GetCollidersInRange(float radius, int layerMask)
        => Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

    public TComponent[] GetComponentsInRange<TComponent>(float radius) where TComponent : MonoBehaviour
    {
        var colliders = GetCollidersInRange(radius);

        var components = colliders.SelectGameObjects().Select(c => c.GetComponent<TComponent>());
        return components.Where(c => c != null).ToArray();
    }

    public bool IsInRange(GameObject obj, float range)
    {
        if (obj == null)
            return false;

        return IsInRange(obj.transform.position, range);
    }

    public bool IsInRange(Transform transform, float radius)
    {
        if (transform == null)
            return false;

        return IsInRange(transform.position, radius);
    }

    public bool IsInRange(Vector3 position, float radius) => transform.position.IsInRange(position, radius);

    public bool IsOutsideRange(GameObject obj, float radius)
    {
        if (obj == null)
            return false;

        return IsOutsideRange(obj.transform.position, radius);
    }

    public bool IsOutsideRange(Vector3 position, float attackRange) => transform.position.IsOutsideRange(position, attackRange);

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough, Conditional("DEBUG")]
    protected void DebugLog(object message, bool condition = true)
        => DebugLog(message, 0, condition);

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough, Conditional("DEBUG")]
    protected void DebugLog(object message, int indentationSpaces, bool condition = true)
    {
        if (!EnableDebugLog || !condition)
            return;

        var spaces = new string(' ', indentationSpaces);
        UnityEngine.Debug.Log($"{spaces}{GetType().FullName} - {message}", this);
    }

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough, Conditional("DEBUG")]
    protected void DebugLogMethodEntry([CallerMemberName] string message = null)
        => DebugLog($"Entry Method - {message}");

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough, Conditional("DEBUG")]
    protected void DebugLogMethodExit([CallerMemberName] string message = null)
        => DebugLog($"Entry Method - {message}");

    protected void Destroy()
        => Destroy(gameObject);

    protected void DestroyTopParent()
        => Destroy(TopParent);

    protected void DisableCollider()
    {
        var colliders = GetComponents<Collider2D>();
        foreach (var c in colliders)
            c.enabled = false;
    }

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    protected TComponent GetInitialisedComponent<TComponent>(ref TComponent innerReference) where TComponent : Component
        => innerReference ?? (innerReference = GetInitialisedComponent<TComponent>());

    [DebuggerHidden, MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    protected TComponent GetInitialisedComponent<TComponent>() where TComponent : Component
    {
        var component = GetComponent<TComponent>();

        Assert.IsNotNull(component, $"Couldn't find component {typeof(TComponent).FullName} on object {gameObject.name}");

        return component;
    }


    #endregion Methods
}