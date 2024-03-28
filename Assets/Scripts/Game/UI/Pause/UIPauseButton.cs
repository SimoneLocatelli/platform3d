using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using static UIPause;

public class UIPauseButton : MonoBehaviour
{
    public OnClickEventHandler OnClick;
    [SerializeField] private UIMenuEnum _commandID;
    [ReadOnlyProperty][SerializeField] private bool _isSelected;
    [SerializeField] private Image _leftCursor;
    [SerializeField] private Image _rightCursor;
    public UIMenuEnum CommandID => _commandID;

    public delegate void OnClickEventHandler(UIMenuEnum commandId);

    /// <summary>
    /// Do not call this method, use <see cref="OnClick"/> to register to events
    /// </summary>
    public void OnButtonClickedCallback()
    {
        OnClick?.Invoke(_commandID);
    }

    public void UpdateSelectedState(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateCursorsState();
    }

    private void Awake()
    {
        _leftCursor = GetUIComponent<Image>("Left Cursor");
        _rightCursor = GetUIComponent<Image>("Right Cursor");
        var button = GetUIComponent<Button>("Button");

        Assert.IsNotNull(_leftCursor);
        Assert.IsNotNull(_rightCursor);
        Assert.IsNotNull(button);

        UpdateCursorsState();
    }

    private TComponent GetUIComponent<TComponent>(string gameObjectName) where TComponent : Component
    {
        var obj = transform.Find(gameObjectName);
        Assert.IsNotNull(obj, $"Couldn't find object {gameObjectName}");
        var component = obj.GetComponent<TComponent>();
        Assert.IsNotNull(component, $"Couldn't find compnent of type {typeof(TComponent)} on object {gameObjectName}");
        return component;
    }

    private void UpdateCursorsState()
    {
        _leftCursor.enabled = _isSelected;
        _rightCursor.enabled = _isSelected;
    }
}