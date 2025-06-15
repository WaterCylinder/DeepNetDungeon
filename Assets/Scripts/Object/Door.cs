using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
装载这个组件的对象在碰撞到玩家时将房间改变到当前房间所链接房间中指定ID的房间
*/

public class Door : MonoBehaviour
{   
    public GameObject _doorPrefab;
    public GameObject doorPrefab => _doorPrefab ? _doorPrefab : _doorPrefab = AssetManager.Load<GameObject>("Door");
    public Room room;
    public bool isClose = false;
    public bool canTrans = true;
    public int index = 0;
    public Room targetRoom;
    public Door targetDoor;
    void OnTriggerEnter2D(Collider2D other){
        if(other.GetComponent<Player>() != null && canTrans && !isClose && targetRoom != null){
            GameBase.now.Trans(targetRoom, targetDoor);
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.GetComponent<Player>() != null){
            canTrans = true;
        }
    }
}
