using UnityEditor;

public interface ICustomEditor
{
    SerializedProperty FindProperty(string propertyName);
}