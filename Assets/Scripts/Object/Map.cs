using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using UnityEngine;

public enum MapState{
    Init,
    MapCreatorDone,
}
[Serializable]
public struct MapRoomList{
    public RoomType type;
    public List<WeightData<string>> roomNames;
    public MapRoomList(RoomType type, List<WeightData<string>> roomNames) {
        this.type = type;
        this.roomNames = roomNames;
    }
    public MapRoomList(RoomType type) {
        this.type = type;
        this.roomNames = new List<WeightData<string>>();
    }
}
public class Map : MonoBehaviour
{   
    //  枚举转int
    public static int FlagValue(MapFlag flag){
        return (int)flag;
    }
    private RogueMapCreator _mapCreator;
    public RogueMapCreator mapCreator{get=>_mapCreator;}
    public MapFlag[,] map{get=>mapCreator.map;}
    public MapState state;
    public int width, height;
    public float size, dispersion;
    public int seed;
    public List<MapRoomList> roomlist;
    // 获取特定type的房间列表
    public MapRoomList FindRoomList(RoomType type) => roomlist.First(x=>x.type == type);
    public List<Room> rooms;
    void Awake(){
        state = MapState.Init;
        if(seed == -1){
            // 随机种子
            seed = Guid.NewGuid().GetHashCode();
        }
        if(size <= 0){
            _mapCreator = new RogueMapCreator(width, height, seed:seed);
        }else{
            _mapCreator = new RogueMapCreator(width, height, size, dispersion, seed:seed);
        }
        //mapCreator.SetSeed(seed);
        mapCreator.Init();
        Debug.Log(mapCreator);
        state = MapState.MapCreatorDone;
    }
    
}
