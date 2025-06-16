using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Bag))]
public class BagEditor : Editor
{
    private Vector2 scrollPos;

    public override void OnInspectorGUI()
    {
        Bag bag = (Bag)target;
        if (bag == null || bag.itemList == null)
        {
            EditorGUILayout.LabelField("Item List", "Empty");
            DrawDefaultInspectorExcept("itemList");
            return;
        }

        // 显示只读的itemList
        string itemListStr = bag.itemList.ToString();
        
        EditorGUILayout.LabelField("Item List");
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, 
            GUILayout.MaxHeight(10 * EditorGUIUtility.singleLineHeight));
        
        GUI.enabled = false;
        EditorGUILayout.TextArea(itemListStr, GUILayout.ExpandHeight(true));
        GUI.enabled = true;
        
        EditorGUILayout.EndScrollView();

        // 绘制其他字段
        DrawDefaultInspectorExcept("itemList");
    }

    /// <summary>
    /// 绘制除指定字段外的所有默认字段
    /// </summary>
    private void DrawDefaultInspectorExcept(params string[] excludedProperties)
    {
        serializedObject.Update();
        var iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        
        while (iterator.NextVisible(enterChildren))
        {
            if (ArrayContains(excludedProperties, iterator.name))
            {
                enterChildren = false;
                continue;
            }
            
            EditorGUILayout.PropertyField(iterator, true);
            enterChildren = false;
        }
        
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 检查数组是否包含指定字符串（忽略大小写）
    /// </summary>
    private bool ArrayContains(string[] array, string value)
    {
        foreach (string item in array)
        {
            if (item.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}