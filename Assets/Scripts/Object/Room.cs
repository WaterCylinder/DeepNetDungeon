using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum RoomType{
    Normal,
    Start,
    End,
    Boss,
} 
public class RoomTag : Tag{
    public static uint None = 0;
}

public class Room : MonoBehaviour
{   
    public RoomType roomType = RoomType.Normal;
    public uint roomId = 0;
    public RoomTag tags = new RoomTag();
    public Vector2 defaultPos;
    public List<Room> links = new List<Room>();
    public List<Door> doors = new List<Door>();
    public void CloseDoor(){
        foreach(Door door in doors){
            door.isClose = true;
        }
    }
    public void OpenDoor(){
        foreach(Door door in doors){
            door.isClose = false;
        }
    }
}
