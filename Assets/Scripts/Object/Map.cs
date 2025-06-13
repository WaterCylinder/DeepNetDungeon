using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapState : Tag{
    public static uint Init,
    MapCreatorDone,
    RoomGenerateDone,
    Ready;
}
[Serializable]
public struct MapRoomList{
    public MapFlag type;
    public List<WeightData<string>> roomNames;
    public MapRoomList(MapFlag type, List<WeightData<string>> roomNames) {
        this.type = type;
        this.roomNames = roomNames;
    }
    public MapRoomList(MapFlag type) {
        this.type = type;
        this.roomNames = new List<WeightData<string>>();
    }
}
public class Map : MonoBehaviour
{   
    public static Map instance;
    private static GameObject _mapPrefab;
    public static GameObject mapPrefab => _mapPrefab ? _mapPrefab : _mapPrefab = AssetManager.Load<GameObject>("Map");
    public static Vector2 defaultSize = new Vector2(10, 10);
    //  枚举转int
    public static int FlagValue(MapFlag flag){
        return (int)flag;
    }
    private RogueMapCreator _mapCreator;
    public RogueMapCreator mapCreator => _mapCreator;
    public MapFlag[,] map => mapCreator.map;
    public MapState state;
    public int width, height;
    public float size, dispersion;
    public int seed;
    public List<MapRoomList> roomlist;
    // 获取特定type的房间列表
    public MapRoomList FindRoomList(MapFlag type) => roomlist.First(x=>x.type == type);
    public List<Room> rooms;
    public System.Random rand;
    void Awake(){
        if(instance != null)Destroy(instance.gameObject);
        instance = this;
        state = new MapState();
        state.Add(MapState.Init);
    }
    public void MapInit(int width, int height, float size = -1, float dispersion = RogueMapCreator.DEFAULT_DISPERSION, int seed = -1){
        this.width = width;
        this.height = height;
        this.size = size;
        this.seed = seed;
    }
    public void MapCreratorInit(){
        if(seed == -1){
            // 随机种子
            seed = Guid.NewGuid().GetHashCode();
        }
        if(size <= 0){
            _mapCreator = new RogueMapCreator(width, height);
        }else{
            _mapCreator = new RogueMapCreator(width, height, size, dispersion);
        }
        mapCreator.SetSeed(seed);
        rand = new System.Random(seed);
        mapCreator.Init();
        Debug.Log(mapCreator);
        state.Add(MapState.MapCreatorDone);
    }
    /// <summary>
    /// 生成房间并添加到map的房间池
    /// </summary>
    /// <param name="roomPrefab"></param>
    /// <param name="pos"></param>
    public void CreateRoom(GameObject roomPrefab, Vector2 pos){
        if(roomPrefab.GetComponent<Room>() == null){
            Debug.LogWarning($"{roomPrefab.name}房间对象没有Room组件");
            return;
        }
        GameObject obj = Instantiate(roomPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(transform);
        rooms.Add(obj.GetComponent<Room>());
    }
    public void CreateRoom(string roomName, Vector2 pos){
        GameBase game = GameManager.instance.game;
        GameObject roomPrefab = game.GetRoom(roomName);
        if(roomPrefab == null){
            Debug.LogWarning($"{roomName}房间不存在");
            return;
        }
        CreateRoom(roomPrefab, pos);
    }
    public void CreateRoom(MapFlag type, Vector2 pos){
        string wpick = null;
        try{
            wpick = Tool.WeightRandomPick(FindRoomList(type).roomNames, rand);
        }catch(Exception e){
            Debug.LogWarning($"{type}房间没有配置{e.Message}");
            return;
        }
        if(wpick == null){
            Debug.LogWarning($"{type}房间没有配置");
            return;
        }
        CreateRoom(wpick, pos);
    }
    public void Generate(){
        Clear();
        for(int i = 0; i < height; i++){
            for(int j = 0; j < height; j++){
                MapFlag flag = map[i, j];
                if(flag == MapFlag.Empty)continue;
                Vector2 pos = new Vector2(j * defaultSize.x, -i * defaultSize.y);
                pos = new Vector2(pos.x - width * defaultSize.x / 2, pos.y + height * defaultSize.y / 2);
                CreateRoom(flag, pos);
            }
        }
        Debug.Log($"房间生成完毕，共生成了{rooms.Count}个房间");
        state.Add(MapState.RoomGenerateDone);
    }
    public void Clear(){
        foreach(Room room in rooms){
            Destroy(room.gameObject);
        }
    }
    
}
