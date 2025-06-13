using System;
using System.IO;
using System.Xml;
using UnityEngine;

public class AssetBundleLoader
{   
    public static string assetPath = AssetPath.assetPath;
    /// <summary>
    /// 普通加载资源包
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static AssetBundle Load(string path){
        return AssetBundle.LoadFromFile(path);
    }
    /// <summary>
    /// 异步加载资源包
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static AssetBundleCreateRequest LoadAsync(string path){
        return AssetBundle.LoadFromFileAsync(path);
    }
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T Load<T>(string path, string name)where T : UnityEngine.Object{
        return AssetBundle.LoadFromFile(Path.Combine(assetPath, path)).LoadAsset<T>(name);
    }
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="complate"></param>
    /// <returns></returns>
    public static Container<T> LoadAsync<T>(string path, string name, Action<T> complate = null)where T : UnityEngine.Object{
        AssetBundleCreateRequest abq = LoadAsync(Path.Combine(assetPath, path));
        Container<T> c = new Container<T>();
        abq.completed += ao => {
            c.Set(abq.assetBundle.LoadAsset<T>(name));
            complate?.Invoke(c.Get());
        };
        return c;
    }
    /// <summary>
    /// 加载资源包中的所有资源
    /// </summary>
    /// <param name="path"></param>
    public static void LoadAll(string path){
        AssetBundle.LoadFromFile(Path.Combine(assetPath, path)).LoadAllAssets();
    }
    /// <summary>
    /// 异步加载资源包中的所有资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="complate"></param>
    public static Container<AssetBundle> LoadAllAsync(string path, Action<AssetBundle> complate = null){
        AssetBundleCreateRequest abq = LoadAsync(Path.Combine(assetPath, path));
        Container<AssetBundle> container = new Container<AssetBundle>();
        abq.completed += ao => {
            AssetBundleRequest req = abq.assetBundle.LoadAllAssetsAsync();
            req.completed += ao => {
                container.Set(abq.assetBundle);
                complate?.Invoke(abq.assetBundle);
            };
        };
        return container;
    }
    /// <summary>
    /// 从加载好的AB包中加载指定资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ab"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T LoadFromAB<T>(AssetBundle ab, string name) where T : UnityEngine.Object{
        return ab.LoadAsset<T>(name);
    }
    /// <summary>
    /// 加载Prefab
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject LoadPrefab(string path, string name){
        AssetBundle ab = Load(Path.Combine(assetPath, path));
        GameObject pfb = ab.LoadAsset<GameObject>(name);
        return pfb;
    }
    /// <summary>
    /// 异步加载prefab
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Container<GameObject> LoadPrefabAsync(string path, string name, Action<GameObject> complate = null){
        AssetBundleCreateRequest abq = LoadAsync(Path.Combine(assetPath, path));
        Container<GameObject> c = new Container<GameObject>();
        abq.completed += ao => {
            c.Set(abq.assetBundle.LoadAsset<GameObject>(name));
            complate?.Invoke(c.Get());
        };
        return c;
    }
    /// <summary>
    /// 加载XML文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static XmlDocument LoadXml(string path, string name){
        AssetBundle ab = Load(Path.Combine(assetPath, path));
        TextAsset text = ab.LoadAsset<TextAsset>(name);
        XmlDocument xml = new XmlDocument();
        xml.LoadXml(text.text);
        return xml;
    }
    /// <summary>
    /// 异步加载XML文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Container<XmlDocument> LoadXmlAsync(string path, string name){
        AssetBundleCreateRequest abq = AssetBundle.LoadFromFileAsync(Path.Combine(assetPath, path));
        Container<XmlDocument> c = new Container<XmlDocument>();
        abq.completed += ao => {
            TextAsset text = abq.assetBundle.LoadAsset<TextAsset>(name);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(text.text);
            c.Set(xml);
        };
        return c;
    }

}
