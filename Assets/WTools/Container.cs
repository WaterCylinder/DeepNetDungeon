using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Container<T>
{   
    private T obj;
    public Container(T obj){
        this.obj = obj;
    }
    public Container(){
        obj = default;
    }
    public void Set(T obj){
        this.obj = obj;
    }
    public T Get(){
        return obj;
    }
}
