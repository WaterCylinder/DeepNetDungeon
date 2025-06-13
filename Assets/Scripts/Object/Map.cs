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
    public const float SIZE_FACTOR = 0.6f;
    public static Map instance;
    private static GameObject _mapPrefab;
    public static GameObject mapPrefab => _mapPrefab ? _mapPrefab : _mapPrefab = AssetManager.Load<GameObject>("Map");
    public static Vector2 defaultSize = new Vector2(20, 20);
    //  枚举转int
    public static int FlagValue(MapFlag flag){
        return (int)flag;
    }
    /// <summary>
    /// 约束位置在范围内
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector2 Limit(Vector2 pos){
        return new Vector2(Mathf.Clamp(pos.x, 0, instance.width - 1), Mathf.Clamp(pos.y, 0, instance.height - 1));
    }
    private RogueMapCreator _mapCreator;
    public RogueMapCreator mapCreator => _mapCreator;
    public MapFlag[,] map => mapCreator.map;
    public MapState state;
    public int height, width;
    public float size, dispersion;
    public int seed;
    public List<MapRoomList> roomlist;
    // 获取特定type的房间列表
    public MapRoomList FindRoomList(MapFlag type) => roomlist.First(x=>x.type == type);
    public Room[,] rooms;
    public int roomNum;
    public System.Random rand;
    void Awake(){
        if(instance != null)Destroy(instance.gameObject);
        instance = this;
        state = new MapState();
        state.Add(MapState.Init);
    }
    public void MapInit(int height, int width, float size = -1, float dispersion = RogueMapCreator.DEFAULT_DISPERSION, int seed = -1){
        this.width = width;
        this.height = height;
        this.size = size;
        this.seed = seed;
    }
    public void MapCreatorInit(){
        if(seed == -1){
            // 随机种子
            seed = Guid.NewGuid().GetHashCode();
        }
        if(size <= 0){
            _mapCreator = new RogueMapCreator(height, width);
        }else{
            _mapCreator = new RogueMapCreator(height, width, size, dispersion);
        }
        mapCreator.SetSeed(seed);
        rand = new System.Random(seed);
        mapCreator.Init();
        state.Add(MapState.MapCreatorDone);
        rooms = new Room[height, width];
    }
    /// <summary>
    /// 带检查的地图初始化
    /// </summary>
    public void MapCreatorInitByCheck(){
        Debug.Log("Map>>尝试生成地图");
        if(seed == -1){
            seed = Guid.NewGuid().GetHashCode();
        }
        seed--;
        int count = 0;
        do{
            seed++;
            count++;
            Debug.Log($"Map检查生成>>尝试{count}次");
            if(count > 10){
                break;
            }
            try{
                MapCreatorInit();
            }catch(Exception e){
                e.GetHashCode();
                continue;
            }
        }while(!MapCreatorCheck());
    }
    /// <summary>
    /// 地图合理性检查
    /// </summary>
    /// <returns></returns>
    public bool MapCreatorCheck(){
        int num = (int)(width * height * mapCreator.size * SIZE_FACTOR);
        Debug.Log($"{num}, {mapCreator.roomNum}");
        //检查房间数量在标准数量-5 5 范围内，并且边界房间数量大于等于三
        if(mapCreator.roomNum > num - 5 &&  mapCreator.roomNum < num + 5
            && mapCreator.ends.Count >= 3){
                return true;
            }
        return false;
    }
    /// <summary>
    /// 生成房间并添加到map的房间池
    /// </summary>
    /// <param name="roomPrefab"></param>
    /// <param name="pos"></param>
    public Room CreateRoom(GameObject roomPrefab, Vector2 pos){
        if(roomPrefab.GetComponent<Room>() == null){
            Debug.LogWarning($"{roomPrefab.name}房间对象没有Room组件");
            return null;
        }
        GameObject obj = Instantiate(roomPrefab, pos, Quaternion.identity);
        obj.transform.SetParent(transform);
        roomNum ++;
        return obj.GetComponent<Room>();
    }
    public Room CreateRoom(string roomName, Vector2 pos){
        GameBase game = GameManager.instance.game;
        GameObject roomPrefab = game.GetRoom(roomName);
        if(roomPrefab == null){
            Debug.LogWarning($"{roomName}房间不存在");
            return null;
        }
        return CreateRoom(roomPrefab, pos);
    }
    public Room CreateRoom(MapFlag type, Vector2 pos){
        string wpick = null;
        try{
            wpick = Tool.WeightRandomPick(FindRoomList(type).roomNames, rand);
        }catch(Exception e){
            Debug.LogWarning($"{type}房间没有配置{e.Message}");
            return null;
        }
        if(wpick == null){
            Debug.LogWarning($"{type}房间没有配置");
            return null;
        }
        return CreateRoom(wpick, pos);
    }
    public void Generate(){
        Debug.Log(mapCreator);
        Clear();
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++){
                MapFlag flag = map[i, j];
                if(flag == MapFlag.Empty)continue;
                Vector2 pos = new Vector2(j * defaultSize.x, -i * defaultSize.y);
                pos = new Vector2(pos.x - width * (int)defaultSize.x / 2, pos.y + height * (int)defaultSize.y / 2);
                //设置房间信息
                Room room = CreateRoom(flag, pos);
                Debug.Log($"创建房间: {room.name} 位置: {pos}");
                rooms[i, j] = room;
                room.mapPos = new Vector2Int(i, j);
            }
        }
        Debug.Log($"房间生成完毕，共生成了{roomNum}个房间");
        state.Add(MapState.RoomGenerateDone);
    }
    public void RoomLink(){
        foreach(Room room in rooms){
            if(room == null)continue;
            Vector2Int pos = room.mapPos;
            Vector2Int[] toward = new Vector2Int[4]{
                new(0, 1),
                new(1, 0),
                new(0, -1),
                new(-1, 0)
            };
            foreach(Vector2Int t in toward){
                Vector2Int tp = pos + t;
                if(mapCreator.Limit(tp.x, tp.y) && map[tp.x, tp.y] != MapFlag.Empty){
                    room.LinkTo(rooms[tp.x, tp.y], t);
                }
            }
            //清除未链接的门
            //TODO 给门加上自清除功能
            for(int i = 0; i< room.doors.Count; i++){
                Door d = room.doors[i].door;
                if(d.targetRoom == null){
                    Destroy(d.gameObject);
                }
            }
        }
    }
    public void Clear(){
        foreach(Room room in rooms){
            if(room != null)Destroy(room.gameObject);
        }
        roomNum = 0;
    }
    
}
