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
    /// 加载指定路径的资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Load<T>(string name)where T : UnityEngine.Object{
        name = Tool.ToPath(name);
        T res = Resources.Load<T>(name);
        string path = Path.GetDirectoryName(name);
        name = Path.GetFileNameWithoutExtension(name);
        if(res == null){
            res = AssetBundleLoader.Load<T>(path, name);
        }
        return res;
    }
    public static T Load<T>(string path, string name)where T : UnityEngine.Object{
        return Load<T>(Path.Combine(path, name));
    }
    public static T LoadFromAB<T>(AssetBundle ab, string name)where T : UnityEngine.Object{
        return AssetBundleLoader.LoadFromAB<T>(ab, name);
    }
    /// <summary>
    /// 加载实体，依赖于Unity的AB包功能，自动缓存已经记载过的AB包。传入资源路径名和资源名。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="signal"></param>
    /// <returns></returns>
    public static GameObject LoadEntity(string name, bool signal = true){
        name = Tool.ToPath(name);
        name = Path.Combine("Entities", name);
        if(signal){
            name = Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name).ToLower() + "_gameobject");
        }
        return Load<GameObject>(name);
    }
    /// <summary>
    /// Game类资源预加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="complate"></param>
    /// <returns></returns>
    public static Container<AssetBundle> PreloadGameAsset(string path, UnityAction<AssetBundle> complate = null){
        path = Tool.ToPath(path);
        Debug.Log($"AssetManager读取路径%StreamAsset%:{path}");
        try{
            return AssetBundleLoader.LoadAllAsync(path, ab=>{complate?.Invoke(ab);});
        }catch(Exception e){
            Debug.LogWarning("预加载资源失败：" + e.Message);
            return Container<AssetBundle>.Done;
        }
        
    }
    /// <summary>
    /// Game类的实体预加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="complate"></param>
    /// <returns></returns>
    public static Container<AssetBundle> PreloadEntity(string path, UnityAction<AssetBundle> complate = null){
        return PreloadGameAsset(Path.Combine("Entities", path), complate);
    }
    /// <summary>
    /// Game类房间预加载
    /// </summary>
    /// <param name="path"></param>
    /// <param name="complate"></param>
    /// <returns></returns>
    public static Container<AssetBundle> PreloadRoom(string path, UnityAction<AssetBundle> complate = null){
        return PreloadGameAsset(Path.Combine("Rooms", path), complate);
    }
}
