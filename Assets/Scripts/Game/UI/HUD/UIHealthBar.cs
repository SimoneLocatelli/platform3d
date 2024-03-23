using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIHealthBar : BaseBehaviourLight
{
    [Header("Settings")]
    public UIHealthBarTypes UIHealthBarType = UIHealthBarTypes.NPC;

    [SerializeField] private Slider _primaryBar;

    [SerializeField] private Slider _secondaryBar;

    [Range(0.1f, 2f)]
    [SerializeField] private float totalUpdateTime = 1;

    [Header("Info")]
    [ReadOnlyProperty]
    [SerializeField] private bool healthChanged;

    [SerializeField] private LifeSystem lifeSystem;

    [SerializeField] private float damageStartTime;

    [ReadOnlyProperty][SerializeField] private float lifePercentage;
    [ReadOnlyProperty][SerializeField] private float primaryBarValue;
    [ReadOnlyProperty][SerializeField] private float secondaryBarValue;
    [ReadOnlyProperty][SerializeField] private float previousSecondaryBarValue;
    [ReadOnlyProperty][SerializeField] private float fUpdateSpeed;

    public enum UIHealthBarTypes
    {
        Player,
        NPC
    }

    private void Start()
    {
        Assert.IsNotNull(_primaryBar);
        Assert.IsNotNull(_secondaryBar);

        if (lifeSystem == null)
        {
            if (UIHealthBarType == UIHealthBarTypes.Player)
                lifeSystem = Blackboards.Instance.PlayerBlackboard.LifeSystem;
            else
                lifeSystem = GetComponentInParent<LifeSystem>();
        }

        Assert.IsNotNull(lifeSystem);

        lifePercentage = lifeSystem.LifePercentage;
        UpdatePrimaryBar(lifePercentage);
        UpdateSecondaryBar(lifePercentage);

        lifeSystem.OnDamageReceived += OnDamageReceived;
        lifeSystem.OnHealed += OnHealed;
    }

    private void OnHealed(LifeSystem lf)
    {
        if (lifeSystem == null)
            lifeSystem = lf;

        UpdatePrimaryBar(lifeSystem.LifePercentage);
        UpdateSecondaryBar(lifeSystem.LifePercentage);

        healthChanged = false;
        damageStartTime = 0;
    }

    private void OnDamageReceived(LifeSystem lf)
    {
        if (lifeSystem == null)
            lifeSystem = lf;

        if (healthChanged)
            UpdateSecondaryBar(primaryBarValue);

        UpdatePrimaryBar(lifeSystem.LifePercentage);

        previousSecondaryBarValue = secondaryBarValue;

        healthChanged = true;
        damageStartTime = 0;
    }

    private void Update()
    {
        if (healthChanged)
            UpdateDamage();

        var transf = transform;
        if (UIHealthBarType == UIHealthBarTypes.NPC)
            RotateHealthBarTowardsCamera(transf);
    }

    private void RotateHealthBarTowardsCamera(Transform transf)
    {
        transf.LookAt(Blackboards.Instance.CameraBlackboard.CameraTransform);
        transf.Rotate(0, 180, 0);
    }

    private void UpdateDamage()
    {
        lifePercentage = lifeSystem.LifePercentage;

        damageStartTime += Time.deltaTime;

        var totalUpdateTimeNormalised = totalUpdateTime;
        damageStartTime = Mathf.Min(damageStartTime, totalUpdateTimeNormalised);

        fUpdateSpeed = FloatRounding.Round(damageStartTime / totalUpdateTimeNormalised, 2);

        if (fUpdateSpeed >= 1)
        {
            UpdateSecondaryBar(lifePercentage);
            healthChanged = false;
            damageStartTime = 0;
        }
        else
        {
            var secBarValue = Mathf.Lerp(previousSecondaryBarValue, lifePercentage, fUpdateSpeed);
            UpdateSecondaryBar(secBarValue);
        }
    }

    private void UpdatePrimaryBar(float lifePercentage)
        => primaryBarValue = _primaryBar.value = lifePercentage;

    private void UpdateSecondaryBar(float lifePercentage)
        => secondaryBarValue = _secondaryBar.value = lifePercentage;
}