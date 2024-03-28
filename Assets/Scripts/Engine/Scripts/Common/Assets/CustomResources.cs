using UnityEngine;
using UnityEngine.Assertions;

public static class CustomResources
{
    public static TComponent InstantiatePrefab<TComponent>(string path) where TComponent : Component
        => InstantiatePrefab(path, Vector3.zero, Quaternion.identity).GetComponent<TComponent>();

    public static GameObject InstantiatePrefab(string path)
        => InstantiatePrefab(path, Vector3.zero, Quaternion.identity);

    public static GameObject InstantiatePrefab(string path, Vector3 position)
        => InstantiatePrefab(path, position, Quaternion.identity);

    public static GameObject InstantiatePrefab(string path, Vector3 position, float zAngle)
        => InstantiatePrefab(path, position, GetQuaternion(zAngle));

    public static GameObject InstantiatePrefab(string path, Vector3 position, Quaternion quaternion)
        => Object.Instantiate(Load(path), position, quaternion);

    public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position)
        => Object.Instantiate(prefab, position, Quaternion.identity);

    public static GameObject InstantiatePrefab(GameObject prefab, Transform transform)
        => Object.Instantiate(prefab, transform.position, Quaternion.identity);

    public static TComponent InstantiatePrefab<TComponent>(GameObject prefab, Vector3 position) where TComponent : Component
        => Object.Instantiate(prefab, position, Quaternion.identity).GetComponent<TComponent>();

    public static TComponent InstantiatePrefab<TComponent>(GameObject prefab, Transform transform) where TComponent : Component
        => Object.Instantiate(prefab, transform.position, Quaternion.identity).GetComponent<TComponent>();

    public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Transform parent)
        => Object.Instantiate(prefab, position, Quaternion.identity, parent);

    public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, float zAngle)
        => Object.Instantiate(prefab, position, GetQuaternion(zAngle));

    public static TComponent InstantiatePrefab<TComponent>(string path, Vector3 start, float angle)
        where TComponent : Component
    {
        var obj = InstantiatePrefab(path, start, angle);
        return obj == null ? null : obj.GetComponent<TComponent>();
    }

    public static TComponent InstantiatePrefab<TComponent>(string path, Vector3 start, Transform parent)
        where TComponent : Component
    {
        var obj = InstantiatePrefab(path, start);

        if (obj == null) return null;

        obj.transform.parent = parent;
        obj.transform.localPosition = start;
        return obj.GetComponent<TComponent>();
    }

    public static TComponent InstantiatePrefab<TComponent>(string path, Vector3 start) where TComponent : Component
    {
        var obj = InstantiatePrefab(path, start);
        return obj == null ? null : obj.GetComponent<TComponent>();
    }

    public static GameObject Load(string path)
                                            => Load<GameObject>(path);

    public static T Load<T>(string path) where T : Object
    {
        CustomAssert.PathHasNoExtension(path, nameof(path));
        var resource = Resources.Load<T>(path);
        Assert.IsNotNull(resource, $"Resource '{path}' of type {typeof(T).FullName} not found or couldn't be loaded.");
        return resource;
    }

    public static T[] LoadAll<T>(string path) where T : Object
    {
        CustomAssert.PathHasNoExtension(path, nameof(path));
        var resource = Resources.LoadAll<T>(path);
        Assert.IsTrue(resource != null, $"Loading resources of type {typeof(T).FullName} from path '{path}' returned null.");
        Assert.IsTrue(resource.Length > 0, $"Loading resources of type {typeof(T).FullName} from path '{path}' returned an empty list.");
        return resource;
    }

    public static Sprite LoadSprite(string path, int index)
    {
        CustomAssert.IsNotNegative(index, nameof(index));
        var resources = LoadAll<Sprite>(path);
        return resources[index];
    }

    public static Sprite LoadSprite(string path)
        => Load<Sprite>(path);

    private static Quaternion GetQuaternion(float zAngle)
        => Quaternion.Euler(0, 0, zAngle);
}