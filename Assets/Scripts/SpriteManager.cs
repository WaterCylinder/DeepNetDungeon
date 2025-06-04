using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

[Serializable]
public struct SpriteAsset{
    public string name;
    public Sprite sprite;
}
public class SpriteManager : DataBase<SpriteAsset>
{
    public static Dictionary<string, SpriteManager> dataBase = new();
    public static string DataBasePath;
    public static void Init(string name = "Base"){ 
        DataBasePath??=Path.Combine(AssetPath.assetPath, Global.Setting("Path", "images"));
        if(!dataBase.ContainsKey(name)){
            SpriteManager db;
            GameObject obj = new GameObject("SpriteDB_" + name);
            db = obj.AddComponent<SpriteManager>();
            DontDestroyOnLoad(obj);
            //初始化SpriteManager
            db.dbname = name;
            db.list = new List<SpriteAsset>();
            db.state = DataBaseState.Init;
            db._path = Path.Combine(DataBasePath, name);
            db.LoadALL();

            dataBase.Add(name, db);
        }
    }
    public static Sprite GetSprite(SpriteManager db, string spritename){
        if(db != null && db.state == DataBaseState.Ready)
            return db.GetSprite(spritename);
        return null;
    }
    public static Sprite GetSprite(string dbname, string spritename){
        if(dataBase.ContainsKey(dbname) && dataBase[dbname].state == DataBaseState.Ready)
            return GetSprite(dataBase[dbname], spritename);
        return null;
    }
    /// <summary>
    /// 使用dbname:spritename的格式读取sprite
    /// </summary>
    /// <param name="spriteinfo"></param>
    /// <returns></returns>
    public static Sprite GetSpriteWith(string spriteinfo){
        string[] parts = spriteinfo.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries)
                              .Select(part => part.Trim())
                              .Where(part => !string.IsNullOrEmpty(part))
                              .ToArray();
        if (parts.Length != 2){
            return GetSpriteInAllDB(spriteinfo);
        }
        return GetSprite(parts[0], parts[1]);
    }
    public static Sprite GetSpriteInAllDB(string spritename){
        foreach(var db in dataBase){
            Sprite i = GetSprite(db.Value, spritename);
            if(i != null){
                return i;
            }
        }
        return null;
    }
    public static bool isReady(string dbname){
        return dataBase.ContainsKey(dbname) && dataBase[dbname].state == DataBaseState.Ready;
    }

    //注意sprite只加载png格式
    public override void LoadALL(){
        if(!Directory.Exists(_path)){
            Directory.CreateDirectory(_path);
            Debug.LogWarning($"{name}目录不存在，已创建。{_path}");
        }
        //只加载png格式的文件
        string[] loadList = Directory.GetFiles(_path).Where(x=>x.EndsWith(".png")).ToArray();
        Debug.Log($"待加载的Sprite列表：{Tool.GetInfo(loadList)}");
        state = DataBaseState.Loading;
        loader = StartCoroutine(LoadAsync(loadList));
    }
    IEnumerator LoadAsync(string[] files){
        foreach(string file in files){
            Debug.Log($"加载Sprite, on:{file}");
            SpriteAsset asset;
            asset.name = Path.GetFileNameWithoutExtension(file);
            asset.sprite = Tool.LoadSpriteFromeFile(file, FilterMode.Point, Global.PIXEL_PERUNIT);
            if(asset.sprite != null)
                list.Add(asset);
            yield return null;
        }
        Debug.Log($"!!!Sprite数据库{name}加载完毕!!!");
        list.Sort((a, b)=>{
            return a.name.CompareTo(b.name);
        });
        state = DataBaseState.Ready;
    }
    public override SpriteAsset Find(string name){
        return Tool.BinaryFind(list, new SpriteAsset(){name = name}, (a, b)=>{
            return a.name.CompareTo(b.name);
        });
    }
    public Sprite GetSprite(string name){
        if(state == DataBaseState.Ready)
            return Find(name).sprite;
        else
            return null;
    }
    
}
