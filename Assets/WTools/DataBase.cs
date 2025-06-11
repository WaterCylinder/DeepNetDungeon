using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DataBaseState{
    Init,
    Loading,
    Ready,
    Error
}
public abstract class DataBase<T> : MonoBehaviour
{
    public Coroutine loader;
    public DataBaseState state;
    [SerializeField]protected string _path;
    public string path{get {return _path;}}
    public string dbname;
    public List<T> list;
    public bool ready{get {return state == DataBaseState.Ready;}}
    public abstract void LoadALL();
    protected abstract IEnumerator LoadAsync(string[] files);
    public abstract T Find(string name);
    void Update(){
        if(state == DataBaseState.Ready){
            if(loader != null)StopCoroutine(loader);
        }
    }
    protected void LoadAsyncByList(string[] loadList){
        string info = typeof(T).Name;
        Debug.Log($"正在加载{info}信息：{loadList}");
        if(loadList.Length <= 0){
            Debug.LogWarning($"目录下没有可加载的文件。");
            state = DataBaseState.Ready;
            return;
        }
        state = DataBaseState.Loading;
        loader = StartCoroutine(LoadAsync(loadList));
    }
    public void ClearALL(){
        list.Clear();
        state = DataBaseState.Init;
        //主动触发GC机制回收未被调用的资源
        GC.Collect();
    }
}
