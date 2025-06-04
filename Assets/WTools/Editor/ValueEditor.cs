# if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(Value))]
public class ValueEditor : PropertyDrawer
{   
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        SerializedProperty baseValue = property.FindPropertyRelative("_baseValue");

        Rect nameRect = new Rect(position.x, position.y, Math.Min(220, position.width * 0.3f), position.height);
        Rect labelRect = new Rect(position.x + nameRect.width, position.y, Math.Min(130, position.width * 0.2f), position.height);
        Rect buttonRect = new Rect(position.x + position.width - 90, position.y, 80, position.height);
        Rect baseValueRect = new Rect(labelRect.x + labelRect.width, position.y, position.width - nameRect.width - labelRect.width - buttonRect.width, position.height);
        

        EditorGUI.LabelField(nameRect, property.displayName);
        EditorGUI.LabelField(labelRect, "基础值：");
        EditorGUI.PropertyField(baseValueRect, baseValue, GUIContent.none);
        if(EditorGUI.DropdownButton(buttonRect, new GUIContent("查看"), FocusType.Keyboard)){
            try{
                UnityEngine.Object obj = property.serializedObject.targetObject;
                Value v = obj.GetType().GetField(property.propertyPath).GetValue(obj) as Value;
                Debug.Log($"{obj.name}_{property.displayName}:\n{v}");
            }catch(Exception e){
                Debug.LogError($"无法定位到变量，仅支持无嵌套Value。Exception:{e}");
            }
        }
    }
    //获取属性高度
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
        return base.GetPropertyHeight(property, label);
    }
}
# endif