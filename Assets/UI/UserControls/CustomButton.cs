using UnityEngine;
using UnityEngine.UIElements;

public class CustomButton : BaseVisualElement
{
    public OnClickEventHandler OnClick;
    private readonly Image leftArrow;
    private readonly Image rightArrow;
    public const string SelectedClassName = "selected";

    public AudioClip ButtonSoundEffect { get; set; }

    public Button Button { get; private set; }

    public UIMenuEnum Command { get; private set; }

    public delegate void OnClickEventHandler(UIMenuEnum commandId);

    public CustomButton(string text, UIMenuEnum command) : base($"ucButton {text}", "ucButton", "horizontal_container")
    {
        Command = command;

        leftArrow = VisualElementExtensions.AddNewElement<Image>(this, $"{text} Left Arrow", "imgArrow", "imgLeftArrow");
        Button = this.AddNewElement<Button>($"{text} Button", "bttDefault");
        Button.text = text;
        Button.clicked += OnClickInternal;
        Button.RegisterCallback<FocusInEvent>(OnFocusGained);
        Button.RegisterCallback<FocusOutEvent>(OnFocusLost);
        rightArrow = this.AddNewElement<Image>($"{text} Right Arrow", "imgArrow", "imgRightArrow");
    }

    private void OnFocusLost(FocusOutEvent evt)
    {
        RemoveFromClassList(SelectedClassName);
    }

    private void OnFocusGained(FocusInEvent evt)
    {
        GameAudioManager.PlayUIElementFocusedSFX();
        AddToClassList(SelectedClassName);
    }

    private void OnClickInternal()
    {
        GameAudioManager.PlayButtonPressedSFX();
        OnClick?.Invoke(Command);
    }

    internal void ManualClick()
        => OnClickInternal();

    internal CustomButton SetArrowSizeToSmall()
    {
        leftArrow.AddToClassList("imgArrowSmall");
        rightArrow.AddToClassList("imgArrowSmall");

        return this;
    }
}