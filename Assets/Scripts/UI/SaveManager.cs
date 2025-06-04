using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool isEmpty = true;
    public string name;
    public DateTime createTime;
    public SaveData()
    {
        isEmpty = true;
    }
    public override string ToString()
    {
        return base.ToString();
    }
}
public class SaveManager : MonoBehaviour
{   
    private static List<SaveData> saves;
    private static int saveIndex = 0;
    public static void ChangeSave(int index){ 
        saveIndex = index;
        Debug.Log("切换存档到" + index);
        if(GetSave() == null){
            SetSave(LoadSave(index));
        }
    }
    
    public static SaveData GetSave(int index){
        if(saves[index] == null){
            saves[index] = new SaveData();
        }
        return saves[index];
    }
    public static SaveData GetSave(){
        return GetSave(saveIndex);
    }
    public static void SetSave(int index, SaveData save){
        saves[index] = save;
    }
    public static void SetSave(SaveData save){
        SetSave(saveIndex, save);
    }

    /// 初始化并加载存档
    void Awake(){
        saves ??= new List<SaveData>();
        LoadSave();
    }

    void LoadSave(){
        //从文件中读取所有存档
        for(int i = 0; i< Global.SAVENUM; i++){
            try{
                Debug.Log("检测到存档已加载:save"+ i + ": " + saves[i].name);
                if(saves[i].isEmpty){
                    Debug.Log("存档为空,重新加载:save"+ i);
                    saves.Add(LoadSave(i));
                }
            }catch(Exception e){
                Debug.Log("未检测到存档:"+ i + e.Message);
                saves.Add(LoadSave(i));
            }
            
        }
    }

    public static SaveData LoadSave(int index){
        if(index >= Global.SAVENUM){
            Debug.Log("指定存档编号大于最大存档编号");
            return null;
        }
        if(index < 0){
            Debug.Log("指定存档编号小于0");
            return null;
        }
        string path = $"{Global.GetPath("savepath")}/save{index}";
        XDocument doc = Tool.ReadXml(path);
        SaveData save = new SaveData();
        if(doc == null){
            Debug.Log($"存档{index}不存在");
            save.isEmpty = true;
            return save;
        }
        save.name = doc.Root.Element("Name").Value;
        DateTime.TryParse(doc.Root.Element("CreateTime").Value,out save.createTime);
        save.isEmpty = false;
        //TODO: 完善存档数据的读取
        return save;
    }

    public static void Save(SaveData save){
        //TODO 存档
        Debug.Log("保存存档: " + save);
    }

    public void ChoiceSave(int index){
        ChangeSave(index);
        Global.ChangeScene("Game");
    }
    public void Back(){
        Global.ChangeScene("MainMenu");
    }
}
