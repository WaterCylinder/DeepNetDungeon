using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameBase : Game
{
    public static new GameBase now{
        get{
            return Game.now as GameBase;
        }
    }
    public Player player;
    public Room room;
    protected virtual void OnStart(){}
    void Start(){
        GameManager.instance.game = this;
        GameStart();
        OnStart();
    }
    public void GameEnd(){
        SaveManager.Save(null);
        StopCoroutine(gameProcess);
    }

    public void GameStart(){
        W = 0;//启动W进程
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
