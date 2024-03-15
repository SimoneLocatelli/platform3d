using System;
using UnityEngine;

[ExecuteInEditMode]
public class LifeSystem : BaseBehaviour
{
    #region Props

    public bool DestroyWhenDead = true;

    [Range(0, 3)]
    public float InvulnerabilityDuration = 1f;

    public delegate void OnDeathEventHandler(LifeSystem lifeSystem);

    public delegate void OnDamageReceivedEventHandler(LifeSystem lifeSystem);

    public bool InvulnerabilityManualControl = false;
    public bool IsInvulnerable;
    public int Life;
    public int MaxLife = 10;

    public event OnDeathEventHandler OnDeath;

    public event OnDamageReceivedEventHandler OnDamageReceived;

    public bool TemporarilyInvulnerableAfterDamage;
    private bool initialised;
    private TimerCooldown InvulnerabilityTimer;

    public bool IsDead { get => Life < 1; }

    public float LifePercentage { get => ((float)Life) / MaxLife; }

    public static void ApplyDamageToObject(GameObject obj, int damage)
    {
        var lifeSystem = obj.GetComponentInChildren<LifeSystem>();
        if (lifeSystem != null) lifeSystem.ApplyDamage(damage);
    }

    [Header("On Death")]
    [SerializeField]
    private bool disableAllColliders = true;

    private bool disableAllMeshRenders = true;

    public bool CanDestroy => DestroyWhenDead && !AudioManager.IsPlaying();

    #endregion Props

    #region Life Cycle

    private void Awake()
    {
        if (initialised) return;

        Life = MaxLife;
        initialised = true;
    }

    private void Update()
    {
        if (IsDead)
        {
            if (DestroyWhenDead && CanDestroy)
                DestroyTopParent();
        }

        if (TemporarilyInvulnerableAfterDamage && IsInvulnerable)
            HandleInvulnerabilityDuration();
    }

    #endregion Life Cycle

    #region Methods

    public bool ApplyDamage(int damage)
    {
        CustomAssert.IsNotNegative(damage, nameof(damage));

        if (IsInvulnerable) return false;

        DebugLog($"Received {damage} damage(s)");
        Life -= damage;
        Life = Life < 0 ? 0 : Life;

        if (IsDead)
            OnDeathOccurred();
        else
        {
            if (!InvulnerabilityManualControl)
                ApplyInvulnerability();

            OnDamageReceived?.Invoke(this);
        }

        return true;
    }

    public void ApplyInvulnerability(Func<bool> blinkWhileTrue = null)
    {
        if (!TemporarilyInvulnerableAfterDamage) return;

        IsInvulnerable = true;

        ResetInvulnerabilityTimer();
    }

    private void HandleInvulnerabilityDuration()
    {
        if (InvulnerabilityTimer.IsReady)
        {
            IsInvulnerable = false;
        }
        InvulnerabilityTimer.Update();
    }

    private void OnDeathOccurred()
    {
        OnDeath?.Invoke(this);

        if (disableAllColliders)
        {
            var colliders = GetComponents<Collider>();

            foreach (var c in colliders)
                c.enabled = false;
        }

        if (disableAllMeshRenders)
        {
            var meshRenderer = GetComponents<MeshRenderer>();

            foreach (var m in meshRenderer)
                m.enabled = false;
        }

        AudioManager.PlayOnDestroySound();
    }

    private void ResetInvulnerabilityTimer()
    {
        if (InvulnerabilityTimer == null)
            InvulnerabilityTimer = new TimerCooldown(InvulnerabilityDuration);
        else
            InvulnerabilityTimer.Reset(InvulnerabilityDuration);
    }

    #endregion Methods
}