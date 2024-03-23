using System;
using UnityEngine;

//[ExecuteInEditMode]
public class LifeSystem : BaseBehaviour
{
    #region Props - Settings

    [Header("Life")]
    [SerializeField] private int _life;

    public int Life
    {
        get => _life;
        private set
        {
            _life = value;
            LifePercentage = CalculateLifePercentage();
        }
    }

    public int MaxLife = 10;

    public bool MaxLifeAtStart = true;

    [Header("Invulnerability")]
    public bool IsInvulnerable;

    [Range(0, 3)]
    public float InvulnerabilityDuration = 1f;

    public bool TemporarilyInvulnerableAfterDamage;

    private TimerCooldown InvulnerabilityTimer;

    public bool InvulnerabilityManualControl = false;

    public bool IsDead { get => Life < 1; }

    #endregion Props - Settings

    #region Props - On Death

    [Header("On Death")]
    [SerializeField] private bool DestroyWhenDead = true;

    [SerializeField] private bool disableAllColliders = true;

    [SerializeField] private bool disableAllMeshRenderers = true;

    [Header("Info")]
    [ReadOnlyProperty]
    [SerializeField] private bool initialised;

    [ReadOnlyProperty]
    [SerializeField] private float _lifePercentage;

    public bool PlayDestroyedSound = false;

    public float LifePercentage
    {
        get => _lifePercentage;
        private set => _lifePercentage = value;
    }

    #endregion Props - On Death

    #region Props

    public bool CanDestroy
        => DestroyWhenDead && (!PlayDestroyedSound || !AudioManager.IsPlaying());

    #endregion Props

    #region Events

    #region OnDeath

    public event OnDeathEventHandler OnDeath;

    public delegate void OnDeathEventHandler(LifeSystem lifeSystem);

    #endregion OnDeath

    #region OnHealed

    public event OnHealedEventHandler OnHealed;

    public delegate void OnHealedEventHandler(LifeSystem lifeSystem);

    #endregion OnHealed

    #region OnDamage

    public event OnDamageReceivedEventHandler OnDamageReceived;

    public delegate void OnDamageReceivedEventHandler(LifeSystem lifeSystem);

    #endregion OnDamage

    #endregion Events

    #region Life Cycle

    private void Awake()
    {
        LifePercentage = CalculateLifePercentage();
        if (initialised) return;

        if (MaxLifeAtStart)
            Heal(MaxLife, true);

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

    public void Heal(int healedHP, bool canResurrect)
    {

        CustomAssert.IsNotNegative(healedHP, nameof(healedHP));

        if (canResurrect && IsDead)
        {
            DebugLog("Resurrecting game object [" + gameObject.name + "]");
        }
        else
        {
            DebugLog("Game object  [" + gameObject.name + "] is dead and cannot be resurrected. Healing skipped");
            return;
        }

        var life = Life + healedHP;
        life = life > MaxLife ? MaxLife : life;

        if (life == Life)
            return;

        Life = life;
        OnHealed?.Invoke(this);
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

        if (disableAllMeshRenderers)
        {
            var meshRenderers = GetComponents<MeshRenderer>();

            foreach (var m in meshRenderers)
                m.enabled = false;

            meshRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (var m in meshRenderers)
                m.enabled = false;
        }

        AudioManager.PlayOnDestroySounds();
    }

    private void ResetInvulnerabilityTimer()
    {
        if (InvulnerabilityTimer == null)
            InvulnerabilityTimer = new TimerCooldown(InvulnerabilityDuration);
        else
            InvulnerabilityTimer.Reset(InvulnerabilityDuration);
    }

    private float CalculateLifePercentage()
    {
        float fLife = Life;
        fLife = fLife / MaxLife;
        fLife = FloatRounding.Round(fLife, 2);
        return fLife;
    }

    #endregion Methods
}