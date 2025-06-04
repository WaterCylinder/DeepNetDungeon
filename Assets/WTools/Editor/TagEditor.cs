# if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(Tag), true)]
public class TagEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        Rect labelRect = new Rect(position.x, position.y, position.width * 0.3f, position.height);
        Rect infoRect = new Rect(position.x + labelRect.width, position.y * 0.7f, position.width * 0.7f, position.height);

        EditorGUI.LabelField(labelRect, property.displayName);

        Tag tags = null;
        try{
            UnityEngine.Object obj = property.serializedObject.targetObject;
            tags = obj.GetType().GetField(property.propertyPath).GetValue(obj) as Tag;
        }catch(Exception e){ e.Equals(e);}

        EditorGUI.TextField(infoRect, tags?.ToString());
    }
    //获取属性高度
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
        return base.GetPropertyHeight(property, label);
    }
}
# endif