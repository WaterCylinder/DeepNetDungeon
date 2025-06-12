using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class AssetManager : MonoBehaviour
{   
    public static void Preload(){
        List<string> loadlist = new();
        string settingPath = Global.Setting("Path", "preload");
        string preloadPath = Path.Combine(AssetBundleLoader.assetPath, settingPath);
        Debug.Log($"正在准备全局预加载资源，on{preloadPath}");
        foreach (var item in Directory.GetFiles(preloadPath)) {
            if(!(item.Contains(".manifest") || item.Contains(".meta")
                || Path.GetFileNameWithoutExtension(item) == Path.GetFileNameWithoutExtension(settingPath)))
                loadlist.Add(item);
        }
        Debug.Log($"全局预加载资源列表：{Tool.GetInfo(loadlist)}");
        foreach (var item in loadlist) {
            Debug.Log($"正在全局预加载资源：{item}");
            AssetBundleLoader.LoadAllAsync(item, o =>{
                Debug.Log($"全局预加载完成：{item}");
            });
        }
    }
    /// <summary>
    /// 加载实体，依赖于Unity的AB包功能，自动缓存已经记载过的AB包。传入资源路径名和资源名。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="signal"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Game类是资源预加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="complate"></param>
    /// <returns></returns>
    public static Container<AssetBundleRequest> PreloadGameAsset(string path, UnityAction<AssetBundleRequest> complate = null){
        path = Tool.ToPath(path);
        Debug.Log($"AssetManager读取路径{path}");
        if(!Directory.Exists(path)){
            Debug.LogWarning("预加载资源失败，路径不存在：" + path);
            return Container<AssetBundleRequest>.Done;
        }
        try{
            return AssetBundleLoader.LoadAllAsync(path, abq=>{complate?.Invoke(abq);});
        }catch(Exception e){
            Debug.LogWarning("预加载资源失败：" + e.Message);
            return Container<AssetBundleRequest>.Done;
        }
        
    }
    /// <summary>
    /// Game类的实体预加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="complate"></param>
    /// <returns></returns>
    public static Container<AssetBundleRequest> PreloadEntity(string path, UnityAction<AssetBundleRequest> complate = null){
        return PreloadGameAsset(Path.Combine("Entities", path), complate);
    }
    
}
