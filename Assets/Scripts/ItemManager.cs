using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
[Serializable]
public class ItemJsonList{
    [SerializeField]
    public List<ItemDTO> list = new();
}
/// <summary>
/// Item数据库
/// </summary>
public class ItemManager : DataBase<Item>
{   
    public static Dictionary<string, ItemManager> dataBase = new();
    public static string DataBasePath;
    /// <summary>
    /// 初始化Item数据库
    /// </summary>
    public static void Init(string name = "Base"){
        DataBasePath ??= Path.Combine(AssetPath.assetPath, Global.Setting("Path", "items"));
        if(!dataBase.ContainsKey(name)){
            ItemManager db;
            GameObject obj = new GameObject("ItemDB_" + name);
            db = obj.AddComponent<ItemManager>();
            DontDestroyOnLoad(obj);
            //初始化ItemManager
            db.dbname = name;
            db.list = new List<Item>();
            db.state = DataBaseState.Init;
            db._path = Path.Combine(DataBasePath, name);
            db.LoadALL();
            dataBase.Add(name, db);
        }

    }
    public static InfoRegex eventRegex = new InfoRegex(new string[]{"Effect", "Event"});
    public static Item.ItemEvent EventMapping(string eventInfo){
        if(string.IsNullOrEmpty(eventInfo)){
            return null;
        }
        var info = eventRegex.Parse(eventInfo);
        if(info == null){
            return null;
        }
        switch(info.Value.type){
            case "Effect":
                return (e, i)=>{
                    Effect.AddEffect(e, info.Value.content, info.Value.args);
                };
            case "Event":
                return (e, i)=>{
                    i.InvokeEvent(info.Value.content, e, info.Value.args);
                };
        }
        return null;
    }
    public static Item GetItem(ItemManager db, string itemname){
        if(db != null && db.state == DataBaseState.Ready)
            return db.Find(itemname);
        return null;
    }
    public static Item GetItem(string dbname, string itemname){
        if(dataBase.ContainsKey(dbname) && dataBase[dbname].state == DataBaseState.Ready)
            return GetItem(dataBase[dbname], itemname);
        return null;
    }
    public static Item GetItemInAllDB(string itemname){
        foreach(var db in dataBase){
            Item i = GetItem(db.Value, itemname);
            if(i != null){
                return i;
            }
        }
        return null;
    }
    public static bool isReady(string dbname){
        return dataBase.ContainsKey(dbname) && dataBase[dbname].state == DataBaseState.Ready;
    }
    private void AddItem(
        string itemName,
        string itemInfo,
        ItemTag itemTag = null,
        string sprite = null,
        Item.ItemEvent OnPick = null,
        Item.ItemEvent OnUse = null,
        Item.ItemEvent OnDrop = null
    ){
        list.Add(new Item(){
            itemName = itemName,
            itemInfo = itemInfo,
            spriteUrl = sprite,
            tags = itemTag,
            OnPick = OnPick,
            OnUse = OnUse,
            OnDrop = OnDrop
        });
    }
    private void AddItem(ItemDTO dto){
        ItemTag tags = new ItemTag();
        tags.Add(tags.GetValue(dto.tags));
        AddItem(
            dto.itemName,
            dto.itemInfo,
            tags,
            dto.spriteUrl,
            EventMapping(dto.OnPick),
            EventMapping(dto.OnUse),
            EventMapping(dto.OnDrop)
        );
    }
    public override void LoadALL(){
        if(!Directory.Exists(_path)){
            Directory.CreateDirectory(_path);
            Debug.LogWarning($"{name}目录不存在，已创建。{_path}");
        }
        string[] loadList = Directory.GetFiles(_path).Where(x=>x.EndsWith(".json")).ToArray();
        LoadAsyncByList(loadList);
    }
    protected override IEnumerator LoadAsync(string[] files){
        foreach(string file in files){
            ItemJsonList dto = JsonTool.Load<ItemJsonList>(file);
            foreach(ItemDTO item in dto.list){
                AddItem(item);
            }
            yield return null;
        }
        //加载完毕
        list.Sort((a, b)=>{
            return a.itemName.CompareTo(b.itemName);
        });
        Debug.Log($"!!!Item数据库{name}加载完毕!!!");
        state = DataBaseState.Ready;
    }

    public override Item Find(string name){
        Item query = new Item();
        query.itemName = name;
        //return list.Find(i => {return i.itemName == name;});
        Item i = Tool.BinaryFind(list, query, (a, b)=>{
            return string.Compare(a.itemName, b.itemName);
        });
        return i;
    }

    public Item GetItem(string name){
        if(state == DataBaseState.Ready)
            return Find(name);
        else
            return null;
    }
}
