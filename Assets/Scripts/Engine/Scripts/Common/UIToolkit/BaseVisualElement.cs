using UnityEngine.Assertions;
using UnityEngine.UIElements;

public abstract class BaseVisualElement : VisualElement
{
    protected BaseVisualElement()
    { }

    protected BaseVisualElement(string name, params string[] classNames)
    {
        this.name = name;
        this.AddToClassList(classNames);
    }
}
