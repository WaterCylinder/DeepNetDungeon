using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

public enum MapState{
    Init,
    MapCreatorDone,
}
public class Map : MonoBehaviour
{   
    private RogueMapCreator _mapCreator;
    public RogueMapCreator mapCreator{get=>_mapCreator;}
    public MapFlag[,] map{get=>mapCreator.map;}
    public MapState state;
    public int width, height;
    public float size, dispersion;
    public int seed;
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
