using System.Collections.Generic;
using System.Linq;


public class CustomButtonsContainer : BaseVisualElement
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    private List<CustomButton> _buttons;

    public IEnumerable<CustomButton> Buttons
    {
        get => _buttons;
        private set => _buttons = value.ToList();
    }

    public CustomButtonsContainer(IEnumerable<CustomButton> buttons, Orientation orientation, bool backgroundVisible, int focusOnButtonIndex = 0) : base("ButtonsContainer", BuildClassList(backgroundVisible))
    {
        if (orientation == Orientation.Horizontal)
            AddToClassList("horizontal_container");
        else
            AddToClassList("vertical_container");

        Buttons = buttons ?? Enumerable.Empty<CustomButton>();

        foreach (var btt in buttons)
            Add(btt);

        FocusButtonAtIndex(focusOnButtonIndex);
    }

    public void InitialiseButtonsTabIndex(int startingIndex)
    {
        foreach (var btt in Buttons)
        {
            btt.Button.tabIndex = startingIndex;
            startingIndex++;
        }
    }

    public CustomButtonsContainer FocusButtonAtIndex(int buttonIndex)
    {
        var buttonToFocusOn = _buttons[buttonIndex].Button;

        buttonToFocusOn.Focus();
        return this;
    }

    public CustomButton GetButton(int index)
        => _buttons[index];

    public CustomButtonsContainer RemoveFocusFromAllButtons()
    {
        _buttons.ForEach(btt => btt.Button.Blur());
        return this;
    }

    public CustomButtonsContainer RemoveFocusFromButtonAtIndex(int buttonIndex)
    {
        _buttons[buttonIndex].Button.Blur();
        return this;
    }

    public CustomButtonsContainer SetArrowSizeToSmall()
    {
        foreach (var btt in Buttons)
            btt.SetArrowSizeToSmall();

        return this;
    }

    private static string[] BuildClassList(bool backgroundVisible)
    {
        var classList = new List<string>
        {
            "buttonsContainer"
        };

        if (backgroundVisible)
            classList.Add("backgroundBorderPrimary");

        return classList.ToArray();
    }

    internal void UpdateButtonsFocusability(bool isFocusable)
    {
        foreach (var btt in Buttons)
            btt.Button.focusable = isFocusable;
    }
}