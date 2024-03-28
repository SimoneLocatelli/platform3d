using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public static class VisualElementExtensions
{
    public static Label AddLabel(this VisualElement parentVisualElement, string text, string className, string name = null)
    {
        Assert.IsNotNull(parentVisualElement);
        var label = parentVisualElement.BuildLabel(text, new[] { className }, name);
        parentVisualElement.Add(label);
        return label;
    }

    public static Label AddLabel(this VisualElement parentVisualElement, string text, IEnumerable<string> classNames, string name = null)
    {
        Assert.IsNotNull(parentVisualElement);
        var label = parentVisualElement.BuildLabel(text, classNames, name);
        parentVisualElement.Add(label);
        return label;
    }

    public static TVisualElement AddNewElement<TVisualElement>(this VisualElement parentVisualElement, string name, params string[] classNames)
        where TVisualElement : VisualElement, new()
    {
        var visualElement = parentVisualElement.BuildNewElement<TVisualElement>(name, classNames);
        parentVisualElement.Add(visualElement);

        return visualElement;
    }

    public static VisualElement AddToClassList<TVisualElement>(this TVisualElement visualElement, IEnumerable<string> classNames)
                  where TVisualElement : VisualElement
    {
        Assert.IsNotNull(visualElement);
        if (classNames == null)
            return visualElement;

        foreach (var cls in classNames)
            visualElement.AddToClassList(cls);

        return visualElement;
    }

    public static Label BuildLabel<TVisualElement>(this TVisualElement visualElement, string text, IEnumerable<string> classNames, string name = null)
        where TVisualElement : VisualElement
    {
        name = string.IsNullOrWhiteSpace(name) ? $"Label {text}" : name;
        var label = new Label
        {
            name = name,
            text = text
        };

        label.AddToClassList(classNames);

        return label;
    }

    public static TVisualElement BuildNewElement<TVisualElement>(this VisualElement parentVisualElement, string name, params string[] classNames)
      where TVisualElement : VisualElement, new()
    {
        var visualElement = new TVisualElement()
        {
            name = name,
        };

        AddToClassList(visualElement, classNames);

        return visualElement;
    }

    public static void LoadStyleSheet<TVisualElement>(this TVisualElement visualElement, string styleSheetName)
        where TVisualElement : VisualElement
    {
        Assert.IsNotNull(visualElement);
        var uss = CustomResources.Load<StyleSheet>(styleSheetName);
        visualElement.styleSheets.Add(uss);
    }
}