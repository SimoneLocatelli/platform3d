using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseBehaviour : BaseBehaviourLight
{
    #region Properties

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

    #region Methods - GetCollidersInRange

    public Collider2D[] GetCollidersInRange(float radius)
        => Physics2D.OverlapCircleAll(transform.position, radius);

    public Collider2D[] GetCollidersInRange(float radius, int layerMask)
        => Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

    #endregion Methods - GetCollidersInRange

    #region Methods - GetComponentsInRange

    public TComponent[] GetComponentsInRange<TComponent>(float radius) where TComponent : MonoBehaviour
    {
        var colliders = GetCollidersInRange(radius);

        var components = colliders.SelectGameObjects().Select(c => c.GetComponent<TComponent>());
        return components.Where(c => c != null).ToArray();
    }

    #endregion Methods - GetComponentsInRange

    #region Methods - IsInRange

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

    #endregion Methods - IsInRange

    #region Methods - IsOutsideRange

    public bool IsOutsideRange(GameObject obj, float radius)
    {
        if (obj == null)
            return false;

        return IsOutsideRange(obj.transform.position, radius);
    }

    public bool IsOutsideRange(Vector3 position, float attackRange) => transform.position.IsOutsideRange(position, attackRange);

    #endregion Methods - IsOutsideRange

    #region Methods - DisableCollider

    protected void Destroy()
        => Destroy(gameObject);

    protected void DestroyTopParent()
        => Destroy(TopParent);

    #endregion Methods - DisableCollider

    #region Methods - Destroy

    protected void DisableCollider()
    {
        var colliders = GetComponents<Collider2D>();
        foreach (var c in colliders)
            c.enabled = false;
    }

    #endregion Methods - Destroy
}