using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    /*string apppath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
    Assembly hotUpdate = Assembly.LoadFrom(apppath + "HotUpdate/HotUpdate.dll");
    Type hello = hotUpdate.GetType("HybirdHello");
    hello.GetMethod("Run").Invoke(null, null);*/
    public TextMeshProUGUI text;
    public GameObject test;
    public bool show = false;
    void Start(){
        //HotUpdateTest();
        //SpriteManager.Init("Base");
        RogueMapCreator map = new RogueMapCreator(9, 9, seed:1219915818);
        map.Init();
        Debug.Log(map);
    }
    void Update(){/*
        if(ItemManager.dataBase.ContainsKey("Base")){
            if(!show){
                Item i = ItemManager.dataBase["Base"].Find("eventtest");
                if(i != null){
                    i?.OnPick.Invoke(null, i);
                    show = true;
                }
            }
        }*/
        /*
        if(SpriteManager.dataBase["Base"].state == DataBaseState.Ready
            && !ItemManager.dataBase.ContainsKey("Base")
        ){
            ItemManager.Init("Base");
        }
        if(!show){
            if(ItemManager.isReady("Base")){
                Item i = ItemManager.GetItem("Base", "itemtest");
                test.GetComponent<SpriteRenderer>().sprite = i.GetSprite();
                show = true;
            }
        }*/
    }
    void HotUpdateTest(){
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
        # if !UNITY_EDITOR

        string apppath = Application.dataPath.Substring(0, Application.dataPath.Length - 7);
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{apppath}/HotUpdate/HotUpdate.dll.bytes"));
        
        # else

        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
        
        #endif

        Type type = hotUpdateAss.GetType("Hello");
        type.GetMethod("Run").Invoke(null, null);
    }
}
