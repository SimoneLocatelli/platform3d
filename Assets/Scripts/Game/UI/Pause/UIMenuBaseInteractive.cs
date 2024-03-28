using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(Canvas))]
public abstract class UIMenuBaseInteractive : UIMenuBase
{
    #region Props Info

    [Header("Buttons")]
    private UIPauseButton[] _buttons;

    [SerializeField][ReadOnlyProperty] private int _selectedButtonIndex = 0;

    protected UIPauseButton[] Buttons
        => _buttons != null ? _buttons : (_buttons = InitButtonsList());

    protected UIPauseButton SelectedButton => Buttons[SelectedButtonIndex];

    private int SelectedButtonIndex
    {
        get => _selectedButtonIndex;
        set
        {
            var buttonsLength = Buttons.Length;

            value %= buttonsLength;
            if (value < 0)
                value += buttonsLength;

            _selectedButtonIndex = value;
        }
    }

    #endregion Props Info

    #region Life Cycle

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(Buttons);
        CustomAssert.IsNotEmpty(Buttons, $"Menu '{TypeName}' - {nameof(Buttons)}");
    }


    #endregion Life Cycle

    #region Methods

    public override void ExitMenu()
    {
        base.ExitMenu();
        SelectedButton.UpdateSelectedState(false);
    }

    public override void OpenMenu()
    {
        base.OpenMenu();
        UpdateSelectedButton(0);
    }

    protected void ActivateSelectedButton()
    {
        var commandId = SelectedButton.CommandID;
        AudioManager.Play($"Confirm pressed");
        OnButtonClicked(commandId);
    }

    protected override bool HandleMenu()
    {
        if (base.HandleMenu())
            return true;

        if (PlayerInputManager.PausePressedDown)
            ExitMenu();
        else if (PlayerInputManager.DownPressedDown)
            MoveCursorDown();
        else if (PlayerInputManager.UpPressedDown)
            MoveCursorUp();
        else if (PlayerInputManager.ConfirmPressedDown)
            ActivateSelectedButton();
        else
            return false;

        return true;
    }

    protected virtual void MoveCursorDown()
    {
        DebugLog($"Menu '{TypeName}' - Moving cursor to next button");
        UpdateSelectedButton(SelectedButtonIndex + 1);
    }

    protected virtual void MoveCursorUp()
    {
        DebugLog($"Menu '{TypeName}' - Moving cursor to previous button");
        UpdateSelectedButton(SelectedButtonIndex - 1);
    }

    protected abstract void OnButtonClicked(UIMenuEnum commandId);

    private UIPauseButton[] InitButtonsList()
    {
        var buttons = transform.GetComponentsInChildren<UIPauseButton>();

        foreach (var button in buttons)
            button.OnClick += OnButtonClicked;

        return buttons;
    }

    private void UpdateSelectedButton(int newSelectedButtonIndex)
    {
        DebugLog($"Menu '{TypeName}' - Button selection changed to index: " + newSelectedButtonIndex);

        SelectedButton.UpdateSelectedState(false);
        SelectedButtonIndex = newSelectedButtonIndex;
        SelectedButton.UpdateSelectedState(true);
        AudioManager.Play($"Button selection changed");
    }

    #endregion Methods
}