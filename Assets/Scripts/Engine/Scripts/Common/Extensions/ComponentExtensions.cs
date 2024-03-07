using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ComponentExtensions
{
    public static T GetComponentFromObjectOrParentOrChildren<T>(this Component component) where T : Component
    {
        var componentResult = component.GetComponent<T>() ?? component.GetComponentInParent<T>();
        return componentResult ?? component.GetComponentInChildren<T>();
    }

    public static GameObject[] SelectGameObjects(this IEnumerable<Component> components)
    {
        if (components == null || !components.Any())
            return new GameObject[0];

        return components.Select(c => c.gameObject).ToArray();
    }


    public static void Destroy(this Component component)
    {
        if (component == null)
            return;

        if (component.gameObject != null)
            Object.Destroy(component.gameObject);
    }
}
