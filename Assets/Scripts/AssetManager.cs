using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetManager : MonoBehaviour
{   
    public static void Preload(){
        List<string> loadlist = new();
        string settingPath = Global.Setting("Path", "preload");
        string preloadPath = Path.Combine(AssetBundleLoader.assetPath, settingPath);
        Debug.Log($"正在准备预加载资源，on{preloadPath}");
        foreach (var item in Directory.GetFiles(preloadPath)) {
            if(!(item.Contains(".manifest") || item.Contains(".meta")
                || Path.GetFileNameWithoutExtension(item) == Path.GetFileNameWithoutExtension(settingPath)))
                loadlist.Add(item);
        }
        Debug.Log($"预加载资源列表：{Tool.GetInfo(loadlist)}");
        foreach (var item in loadlist) {
            Debug.Log($"正在预加载资源：{item}");
            AssetBundleLoader.LoadAllAsync(item, o =>{
                Debug.Log($"预加载完成：{item}");
            });
        }
    }
    public static GameObject LoadEntity(string name, bool signal = true){
        name = Tool.ToPath(name);
        string path = Path.GetDirectoryName(name);
        name = Path.GetFileNameWithoutExtension(name);
        GameObject pb = Resources.Load<GameObject>(Path.Combine("Entities", name));
        if(pb == null){
            string p = signal ?
                Path.Combine("Entities", path, name.ToLower() + "_gameobject")
                : Path.Combine("Entities", path);
            pb = AssetBundleLoader.LoadPrefab(p, name);
        }
        return pb;
    }
}
