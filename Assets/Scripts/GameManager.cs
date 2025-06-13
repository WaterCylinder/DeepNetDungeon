using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
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
    public static GameManager ins => instance;
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

    public Entity EntityCreate(string name, Vector2 postion){
        GameObject obj = AssetManager.LoadEntity(name);
        Entity ent = Instantiate(obj, postion, Quaternion.identity).GetComponent<Entity>();
        EntityAdd(ent);
        return ent;
    }

    public void EntityDestroy(Entity entity){
        entityPool.Remove(entity);
        Destroy(entity.gameObject);
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
 