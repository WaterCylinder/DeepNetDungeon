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
    public abstract void LoadALL();
    public abstract T Find(string name);
    void Update(){
        if(state == DataBaseState.Ready){
            if(loader != null)StopCoroutine(loader);
        }
    }
    public void ClearALL(){
        list.Clear();
        state = DataBaseState.Init;
        //主动触发GC机制回收未被调用的资源
        GC.Collect();
    }
}
