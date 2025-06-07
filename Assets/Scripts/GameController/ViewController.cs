using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
* 控制游戏内一些演出场景
*/

public class ViewController : MonoBehaviour
{
    public Game game;
    public Coroutine viewProcess;
    public GameObject view;
    public float viewFadeTime = 0.5f;


    public List<Sprite> Images;


    public Vector2 countOfStartView;

    [SerializeField]
    private int isEnter = 0;//输入Enter设为2，归零说明输入所有派生事件结束
    [SerializeField]
    private int count;

    void Start(){
        if(game == null){
            game = GameManager.instance.game;
        }
    }
    /// <summary>
    /// 开始一个演出进程，进程会自动配置关闭时点
    /// </summary>
    /// <param name="processName"></param>
    /// <param name="callback"></param>
    public void StartProcess(string processName, UnityAction callback = null){
        count = -1;
        view.SetActive(true);
        switch(processName){
            case "Start":
                viewProcess = StartCoroutine(ViewProcess_Start(callback));
                break;
        }
    }
    void EndProcess(){
        StopCoroutine(viewProcess);
        view.SetActive(false);
    }
    public IEnumerator ViewProcess_Start(UnityAction callback = null){//初始演出
        while(true){
            if(count < 0)
                count = (int)countOfStartView.x;
            Image image = view.transform.Find("Image").GetComponent<Image>();//获取图片对象
            if(image.sprite != Images[count]){
                image.color = new Color(1,1,1,0);
                image.sprite = Images[count];
            }
            if(image.color.a < 1f && isEnter != 1){
                image.color = new Color(1,1,1,Mathf.Min(1f,image.color.a+ (1 / (Global.VIEW_FPS * viewFadeTime))));
            }else{
                if(isEnter > 0){
                    isEnter = 1;
                    image.color = new Color(1,1,1,Mathf.Max(0,image.color.a - (1 / (Global.VIEW_FPS * viewFadeTime))));
                    if(image.color.a <= 0f){
                        isEnter = 0;
                        count++;
                    }
                }
            }
            if(count > (int)countOfStartView.y){
                callback?.Invoke();
                EndProcess();
                yield return null;
            }
            yield return new WaitForSeconds(1f / Global.VIEW_FPS);
        }
    }

    void Update(){
        InputController();
    }
    void InputController(){
        if(InputManager.Enter()){
            isEnter = 2;
        }
    }

}
