using UnityEngine;
using UnityEngine.Assertions;

public class AttackCollider : BaseBehaviour
{
    [Range(1f, 100f)]
    [SerializeField]
    private int damage;

    [SerializeField]
    private Collider swordCollider;

    [Header("Debug")]
    [SerializeField]
    [ReadOnlyProperty]
    private bool canApplyDamage = true;

    #region Events

    private OnDamageAppliedHandler onDamageApplied;

    public delegate void OnDamageAppliedHandler(GameObject objectHit);

    public event OnDamageAppliedHandler OnDamageApplied
    {
        add { onDamageApplied += value; }
        remove { onDamageApplied -= value; }
    }

    #endregion Events

    public void ToggleCollider(bool isEnabled)
    {
        if (swordCollider == null)
            swordCollider = GetComponent<Collider>();

        Assert.IsNotNull(swordCollider);
        DebugLog("Collider enabled state toggled: " + isEnabled, swordCollider.enabled != isEnabled);
        swordCollider.enabled = isEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        DebugLog("Collision detected with trigger object " + other.name);
        var damagedApplied = LifeSystemHandler.ApplyDamage(other, damage);

        if (damagedApplied)
            onDamageApplied?.Invoke(other.gameObject);
    }
}