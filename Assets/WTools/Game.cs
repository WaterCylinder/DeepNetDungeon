using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Game : MonoBehaviour
{   
    public const float GAMEPROCESS_TIMESTEP = 0.1f;
    public static Game now;
    /// <summary>
    /// 游戏流程变量
    /// </summary>u
    public int W;
    public Coroutine gameProcess;
    public DataBaseState assetState;

    void Awake(){
        now = this;
        W = -1;
        gameProcess = StartCoroutine(Process());
    }

    IEnumerator Process(){
        while(true){
            WController();
            yield return new WaitForSeconds(GAMEPROCESS_TIMESTEP);
        }
    }

    protected abstract void WController();
    
    public void Next(){
        W+=10;
    }
    public void Next(int step){
        W+=step;
    }
    public void Goto(int target){
        W = target;
    }
}
