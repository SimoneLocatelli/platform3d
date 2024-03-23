using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIStamina : BaseBehaviour
{
    private UnityEngine.UI.Slider _slider;

    private UnityEngine.UI.Slider Slider => GetInitialisedComponent(ref _slider);

    private void Start()
    {
        Assert.IsNotNull(Slider);
    }

    private void Update()
    {
        Slider.value = Blackboards.Instance.PlayerBlackboard.StaminaPercentage;
    }
}