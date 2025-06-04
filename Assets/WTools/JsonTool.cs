using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public static class JsonTool
{
    public static T Load<T>(string filePath)
    {
        try{
            if (File.Exists(filePath)){
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<T>(json);
            }
            Debug.LogWarning($"文件不存在: {filePath}");
            return default(T);
        }
        catch (System.Exception ex){
            Debug.LogWarning($"JSON加载失败: {ex.Message}");
            return default(T);
        }
    }

    public static void Save<T>(string filePath, string name, T data)
    {   
        try{
            filePath = Path.Combine(Tool.ToPath(filePath), name);
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory)){
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"保存成功：{json}");
        }
        catch (System.Exception ex){
            Debug.LogWarning($"JSON保存失败: {ex.Message}");
        }
    }
}