using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UIStamina : BaseBehaviour
{

    private UnityEngine.UI.Slider _slider;

    UnityEngine.UI.Slider Slider => GetInitialisedComponent(ref _slider);

    [SerializeField]
    private PlayerController playerController;

    private void Start()
    {
        Assert.IsNotNull(_slider);
        Assert.IsNotNull(playerController);
    }

    private void Update()
    {
        Slider.value = playerController.StaminaPercentage;
    }
}