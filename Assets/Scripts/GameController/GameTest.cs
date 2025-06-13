using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTest : GameBase
{   

    # region 固定对象

    public GameInsideMenu menu;
    public ViewController view;
    public TextMeshProUGUI debugger;
    public SpriteRenderer debugRenderer;

    # endregion

    public Entity test;

    protected override void OnStart(){
        DebugOnStart();
    }

    void Update(){
        InputController();
    }
    
    /// <summary>
    /// W流程控制
    /// </summary>
    protected override void WController(){
        switch (W){
            case 0:
                Debug.Log("W0: 游戏开始");
                //初始化游戏
                //生成地图
                Debug.Log(roomABContainer);
                GenerateMap();
                map.MapInit(5, 10);
                map.MapCreatorInitByCheck();
                map.Generate();
                map.RoomLink();
                Trans(map.rooms[map.mapCreator.start.x, map.mapCreator.start.y]);
                Next();
                Goto(100);//暂时不考虑动画
                break;
            case 10:
                Debug.Log("W10: 开始初始动画");
                //插入动画
                //使用演出进程创建初始演出进程
                //view.StartProcess("Start");
                Next();
                break;
            case 20:
                Debug.Log("W20: 播放初始动画中");
                Next();
                break;
            case 30:
                Debug.Log("W30: 播放初始动画结束");
                Goto(100);
                break;
            case 100:
                Debug.Log("W100: 进入游戏前");
                DebugOn100();
                OnGameSatrt();
                Next();
                break;
            case 110:
                Debug.Log("W110: 游戏中");
                OnGame();
                break;

            case -100:
                Debug.Log("W-100: 加载中");
                break;
        }
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
    
    public void DebugOnStart(){
        //Goto(100);
        //test = GameManager.instance.EntityCreate("EnemyTest2", new Vector2(2, 2.45f));
        //test.AddEffect("EffectTest");
        /*SpriteManager.Init("Base");
        ItemManager.Init("Base");*/
    }
    public void DebugOn100(){
        /*player.bag.AddItem("itemtest");
        debugRenderer.sprite = player.bag.GetItem("itemtest").GetSprite();*/
    }
    public void OnGameSatrt(){

    }
    public void OnGame(){

    }
}
