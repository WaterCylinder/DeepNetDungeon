using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBase : Game
{
    public string gameName;
    public static new GameBase now{
        get{
            return Game.now as GameBase;
        }
    }
    public Player player;
    public Map map;
    public Room room;
    public ItemManager itemManager;
    public SpriteManager spriteManager;
    public Container<AssetBundle> entityABContainer;
    public Container<AssetBundle> assetABContainer;
    public Container<AssetBundle> roomABContainer;
    protected virtual void OnStart(){}
    void Start(){
        Game.now = this;
        GameStart();
        LoadAsset();
        OnStart();
    }
    public void GameEnd(){
        SaveManager.Save(null);
        itemManager?.ClearALL();
        spriteManager?.ClearALL();
        StopCoroutine(gameProcess);
    }

    public void GameStart(){
        W = 0;//启动W进程
    }
    //加载资源
    protected override void AssetLoaderCheck(){
        if(spriteManager == null){
            entityABContainer = null;
            assetABContainer = null;
            roomABContainer = null;
            Debug.Log("开始加载sprite资源");
            SpriteManager.Init(gameName);
            spriteManager = SpriteManager.dataBase[gameName];
            return;
        }
        if(spriteManager.ready){
            if(itemManager == null){
                Debug.Log("开始加载item信息");
                ItemManager.Init(gameName);
                itemManager = ItemManager.dataBase[gameName];
                return;
            }
            if(assetABContainer == null){
                Debug.Log("开始加载其他资源");
                assetABContainer = AssetManager.PreloadGameAsset($"Game/{gameName.ToLower()}");
                return;
            }
            if(itemManager.ready && assetABContainer.done){
                if(entityABContainer == null){
                    Debug.Log("其他资源与Item信息加载完毕");
                    Debug.Log("开始加载Entity资源");
                    entityABContainer = AssetManager.PreloadEntity($"GameAssets/{gameName.ToLower()}");
                    return;
                }
                if(roomABContainer == null){
                    Debug.Log("开始加载Room资源");
                    roomABContainer = AssetManager.PreloadRoom($"{gameName.ToLower()}");
                    return;
                }
                if(entityABContainer.done && roomABContainer.done){
                    Debug.Log("Game资源加载完毕");
                    EndLoadAsset();
                }
            }
        }
    }
    public T GetAsset<T>(AssetBundle ab, string name)where T : UnityEngine.Object{ 
        return AssetManager.LoadFromAB<T>(ab, name);
    }
    public GameObject GetEntity(string name) => GetAsset<GameObject>(entityABContainer.Get(), name);
    public GameObject GetRoom(string name) => GetAsset<GameObject>(roomABContainer.Get(), name);
    public GameObject GetAsset(string name) => GetAsset<GameObject>(assetABContainer.Get(), name);

    protected void GenerateMap(){
        GameObject obj = Instantiate(Map.mapPrefab, transform.position, Quaternion.identity);
        map = obj.GetComponent<Map>();
    }
    protected override void LoadAsset(){
        GotoTemp(-100);
        base.LoadAsset();
    }
    protected override void EndLoadAsset(){
        base.EndLoadAsset();
        GotoTemp();
    }

    /// <summary>
    /// 传送到指定房间，并更新房间信息
    /// </summary>
    /// <param name="room"></param>
    /// <param name="index"></param>
    public void Trans(Room room, int index = 0){
        if(room != null){
            this.room = room;
            Vector2 pos;
            if(index >= 0){
                pos = room.doors[index].transform.position;
                room.doors[index].canTrans = false;
            }else{
                pos = (Vector2)room.transform.position + room.defaultPos;
            }
            player.transform.position = pos;
        }
        
    }
}
