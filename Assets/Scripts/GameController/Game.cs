using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{   
    public static Game instance;
    /// <summary>
    /// 游戏流程变量
    /// </summary>u
    public int W;
    public Coroutine gameProcess;

    # region 固定对象

    public GameInsideMenu menu;
    public ViewController view;
    public TextMeshProUGUI debugger;
    public SpriteRenderer debugRenderer;

    # endregion

    public Player player;
    public Room room;
    public Entity test;

    void Awake(){
        instance = this;
        W = -1;
        gameProcess = StartCoroutine(Process());
        GameManager.instance.game = this;
    }
    void Start(){
        GameStart();
        DebugOnStart();
    }
    void Update(){
        InputController();
    }
    IEnumerator Process(){
        while(true){
            WController();
            yield return new WaitForSeconds(Global.GAMEPROCESS_TIMESTEP);
        }
    }
    /// <summary>
    /// W流程控制
    /// </summary>
    void WController(){
        switch (W)
        {
            case 0:
                Debug.Log("W0: 游戏开始");
                //初始化游戏
                Next();
                break;
            case 10:
                Debug.Log("W1: 开始初始动画");
                //插入动画
                //使用演出进程创建初始演出进程
                view.StartProcess("Start");
                Next();
                break;
            case 20:
                Debug.Log("W2: 播放初始动画中");
                Next();
                break;
            case 30:
                Debug.Log("W3: 播放初始动画结束");
                Goto(100);
                break;
            case 100:
                Debug.Log("W10: 进入游戏");
                DebugOn100();
                Next();
                break;
            case 110:
                Debug.Log("W11: 游戏中");
                break;
        }
    }
    public void Next(){
        W+=10;
    }
    public void Next(int step){
        W+=step;
    }
    public void Goto(int target){
        W = target;
    }
    /// <summary>
    /// 全局输入控制
    /// </summary>
    void InputController(){
        if(InputManager.ESC()){
            if(menu.isOpen){
                menu.BackToGame();
            }else{
                menu.OpenMenu();
            }
        }
    }
    /// <summary>
    /// 传送到指定房间，并更新房间信息
    /// </summary>
    /// <param name="room"></param>
    /// <param name="index"></param>
    public void Trans(Room room, int index = 0){
        if(room != null){
            this.room = room;
            Vector2 pos;
            if(index >= 0){
                pos = room.doors[index].transform.position;
                room.doors[index].canTrans = false;
            }else{
                pos = (Vector2)room.transform.position + room.defaultPos;
            }
            player.transform.position = pos;
        }
        
    }
    public void GameStart(){
        W = 0;//启动W进程
    }
    public void GameEnd(){
        SaveManager.Save(null);
        StopCoroutine(gameProcess);
    }
    public void DebugOnStart(){
        Goto(100);
        test = GameManager.instance.EntityCreate("EnemyTest2", new Vector2(2, 2.45f));
        //test.AddEffect("EffectTest");
        /*SpriteManager.Init("Base");
        ItemManager.Init("Base");*/
    }
    public void DebugOn100(){
        /*player.bag.AddItem("itemtest");
        debugRenderer.sprite = player.bag.GetItem("itemtest").GetSprite();*/
    }
}
