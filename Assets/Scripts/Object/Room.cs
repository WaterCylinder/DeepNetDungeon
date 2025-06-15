using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomTag : Tag{
    public static uint None = 0,
    Enter,
    IsClear;
}
[Serializable]
public struct DoorInfo{
    public Vector2Int toward;
    public Door door;
}

public class Room : MonoBehaviour
{   
    public MapFlag roomType = MapFlag.Normal;
    public uint roomId = 0;
    public Vector2Int mapPos;
    public RoomTag tags = new RoomTag();
    public Vector2 defaultPos;
    public List<Room> links = new();
    public List<DoorInfo> doors = new();
    public List<EnemyGenerator> enemyGenerators = new();//敌人生成器
    public Transform enemyPool;
    public List<Enemy> enemies = new();//敌人单位
    void Awake(){
        enemyPool = enemyPool ? enemyPool : transform.Find("EnemyPool");
    }
    public void Init(){
        enemyGenerators = new();
        enemies = new();
        foreach(Transform chi in enemyPool){
            if(chi.GetComponent<EnemyGenerator>() != null)
                enemyGenerators.Add(chi.GetComponent<EnemyGenerator>());
        }
    }
    public Door GetDoor(Vector2Int toward){
        return doors.Find(x => x.toward == toward).door;
    }
    public void LinkTo(Room other, Vector2Int toward, bool toDoor = true){
        Door door = GetDoor(toward);
        door.targetRoom = other;
        door.targetDoor = toDoor ? other.GetDoor(-toward) : null;
        links.Add(other);
    }
    public void CloseDoor(){
        foreach(DoorInfo door in doors){
            door.door.isClose = true;
        }
    }
    public void OpenDoor(){
        foreach(DoorInfo door in doors){
            door.door.isClose = false;
        }
    }
    /// <summary>
    /// 创建敌人实体对象
    /// </summary>
    /// <param name="enemyName"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Enemy CreatEnemy(string enemyName, Vector2 pos){
        GameObject obj = GameManager.instance.game.GetEntity(enemyName);
        if(obj == null)
            obj = AssetManager.LoadEntity(enemyName);
        Enemy enm = (Enemy)GameManager.instance.EntityCreate(obj, pos);
        if(enm == null)return null;
        enm.room = this;
        enm.transform.SetParent(enemyPool);
        enemies.Add(enm);
        return enm;
    }
    public void GenarateEnemy(){
        foreach(EnemyGenerator gen in enemyGenerators){
            if(gen != null){
                gen.Generate();
            }
        }
    }
    void Update(){
        if(tags.Check(RoomTag.Enter) 
            && !tags.Check(RoomTag.IsClear) 
            && enemies.Count <= 0){
            OpenDoor();
            tags.Add(RoomTag.IsClear);
        }
    }
    public void EnterRoom(){
        tags.Add(RoomTag.Enter);
        if(tags.Check(RoomTag.IsClear)){
            OpenDoor();
        }else{
            CloseDoor();
            GenarateEnemy();
        }
    }
    public void ExitRoom(){

    }
}
