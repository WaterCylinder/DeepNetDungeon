using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
装载这个组件的对象在碰撞到玩家时将房间改变到当前房间所链接房间中指定ID的房间
*/

public class Door : MonoBehaviour
{   
    public Room room;
    public bool isClose = false;
    public bool canTrans = true;
    public int index = 0;
    public int targetRoomIndex = -1;
    public int targetDoorIndex = -1;
    void Start(){
        if(targetRoomIndex >= 0 && room.links[targetRoomIndex] == null){
            targetRoomIndex = -1;
        }
    }
    void OnTriggerEnter2D(Collider2D other){
        if(other.GetComponent<Player>() != null && canTrans && !isClose && targetRoomIndex != -1){
            Game.instance.Trans(room.links[targetRoomIndex], targetDoorIndex);
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.GetComponent<Player>() != null){
            canTrans = true;
        }
    }
}
