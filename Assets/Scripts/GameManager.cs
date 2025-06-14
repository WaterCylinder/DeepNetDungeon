using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    # region 全局映射

    public static GameBase G_game => instance.game;
    public static Player G_player => instance.game.player;
    public static Vector2 G_playerPos => G_player.transform.position;
    public static GameManager G_ins => instance;

    # endregion
    private static GameManager _instance;
    public static GameManager instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType<GameManager>();
                if(_instance == null){
                    GameObject gameManager = new GameObject("GameManager");
                    _instance = gameManager.AddComponent<GameManager>();
                    DontDestroyOnLoad(gameManager);
                }
            }
            return _instance;
        }
    }
    /// <summary>
    /// 切换暂停游戏
    /// </summary>
    public static void Pause(){
        if(instance.opera > 0){
            instance.opera = 0;
        }else{
            instance.opera++;
        }
        Time.timeScale = CanOpera ? 1 : 0;
    }
    /// <summary>
    /// 改变是否可操作
    /// </summary>
    /// <param name="op"></param>
    public static void Opera(int op){
        instance.opera+=op;
    }
    public static bool CanOpera{
        get{
            return instance.opera <= 0;
        }
    }
    public static Coroutine mainProcess;
    public SkipList<Entity> entityPool = new SkipList<Entity>();
    public GameBase game => (GameBase)Game.now;
    public SkipList<DropItem> dropItemPool = new SkipList<DropItem>();
    public int opera = 0;
    /// <summary>
    /// 初始化游戏
    /// </summary>
    public void Init(){
        //加载配置
        Debug.Log("加载配置");
        Global.LoadSetting();
    }
    void Start(){
        mainProcess ??= StartCoroutine(MainProcess());
        DebugTest();
        AssetManager.Preload();
    }
    //主进程
    private float timer_mainprocess;
    IEnumerator MainProcess(){
        while(true){
            timer_mainprocess += Global.MAINPROCESS_TIMESTEP;
            if(timer_mainprocess > 1f){
                Debug.Log("- MainProcee runing - ");
                timer_mainprocess = 0f;
            }
            yield return new WaitForSeconds(Global.MAINPROCESS_TIMESTEP);
        }
    }

    public void EntityAdd(Entity entity){
        entityPool.Add(entity, true, false);
    }

    /// <summary>
    /// 创建实体并加入到对象池中
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="postion"></param>
    /// <returns></returns>
    public Entity EntityCreate(GameObject prefab, Vector2 postion){
        try{
            Entity ent = Instantiate(prefab, postion, Quaternion.identity).GetComponent<Entity>();
            EntityAdd(ent);
            return ent;
        }catch(System.Exception e){
            Debug.LogError(e);
            return null;
        }
    }

    public Entity EntityCreate(string name, Vector2 postion){
        GameObject obj = AssetManager.LoadEntity(name);
        return EntityCreate(obj, postion);
    }

    public void EntityDestroy(Entity entity){
        entityPool.Remove(entity);
        Destroy(entity.gameObject);
    }

    public void ItemDrop(Item item, Vector2 pos){
        try{
            DropItem di = Instantiate(DropItem.prefab, pos, Quaternion.identity).GetComponent<DropItem>();
            di.item = item;
            di.Init();
            dropItemPool.Add(di, true, false);
        }catch(System.Exception e){
            Debug.LogWarning(e);
        }
    }
    public void ItemRemove(DropItem dropItem){
        try{
            dropItemPool.Remove(dropItem);
        }catch(System.Exception e){
            Debug.LogWarning(e);
        }
    }

    void DebugTest(){
        
        /*
        Value v = new Value(0);
        v.AddAffect(1.2f, "test");
        Debug.Log(v);

        GameObject obj = AssetManager.LoadEntity("EntityTest");
        Instantiate(obj);

        XmlDocument xml = AssetManager.LoadEntityConfig("EntityTest");
        Debug.Log(xml.InnerXml);*/
    }
}
 