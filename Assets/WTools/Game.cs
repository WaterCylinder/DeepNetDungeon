using System.Collections;
using UnityEngine;

public abstract class Game : MonoBehaviour
{   
    public const float GAMEPROCESS_TIMESTEP = 0.1f;
    public static Game now;
    /// <summary>
    /// 游戏流程变量
    /// </summary>u
    public int W;
    protected int WTemp;
    public Coroutine gameProcess;
    public Coroutine assetProcess;
    public DataBaseState assetState;

    protected abstract void WController();
    protected abstract void AssetLoaderCheck();


    void Awake(){
        now = this;
        W = -1;
        gameProcess = StartCoroutine(Process());
        LoadAsset();
        assetState = DataBaseState.Init;
    }

    protected virtual void LoadAsset(){
        assetProcess = StartCoroutine(AssetLoader());
    }

    protected virtual void EndLoadAsset(){
        if(assetProcess != null)StopCoroutine(assetProcess);
        assetState = DataBaseState.Ready;
    }

    IEnumerator Process(){
        while(true){
            WController();
            yield return new WaitForSeconds(GAMEPROCESS_TIMESTEP);
        }
    }
    IEnumerator AssetLoader(){
        while(assetState != DataBaseState.Ready){
            assetState = DataBaseState.Loading;
            AssetLoaderCheck();
            yield return new WaitForSeconds(GAMEPROCESS_TIMESTEP);
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
    public void GotoTemp(int target){
        WTemp = W;
        W = target;
    }
    public void GotoTemp(){
        W = WTemp;
    }
}
