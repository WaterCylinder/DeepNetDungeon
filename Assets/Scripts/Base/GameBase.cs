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
    public Room room;
    public ItemManager itemManager;
    public SpriteManager spriteManager;
    public Container<AssetBundleRequest> entityRequestContainer;
    public Container<AssetBundleRequest> assetRequestContainer;
    protected virtual void OnStart(){}
    void Start(){
        GameManager.instance.game = this;
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
            entityRequestContainer = null;
            assetRequestContainer = null;
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
            if(assetRequestContainer == null){
                Debug.Log("开始加载其他资源");
                assetRequestContainer = AssetManager.PreloadGameAsset($"Game/{gameName}");
                return;
            }
            if(itemManager.ready && assetRequestContainer.done){
                if(entityRequestContainer == null){
                    Debug.Log("其他资源与Item信息加载完毕");
                    Debug.Log("开始加载Entity资源");
                    entityRequestContainer = AssetManager.PreloadEntity($"GameAssets/{gameName}");
                    return;
                }
                if(entityRequestContainer.done){
                    Debug.Log("Game资源加载完毕");
                    EndLoadAsset();
                }
            }
        }
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
