using System;
using UnityEngine;

public class ItemTag : Tag{
    public static uint None = 0;
    public static uint Active;//1
    public static uint Instant;//2
    public static uint Passive;//4
}
[Serializable]
public class ItemDTO{
    public string itemName;
    public string itemInfo;
    public string spriteUrl;
    public string tags;
    public string OnPick;
    public string OnUse;
    public string OnDrop;
    public ItemDTO(){}
}
[Serializable]
/// <summary>
/// AOP的实体类，类数据库的实现
/// </summary>
public class Item
{   
    public delegate void ItemEvent(Entity entity, Item item);
    public string itemName;
    public string itemInfo;
    public string spriteUrl;
    public ItemTag tags;
    public ItemEvent OnPick;
    public ItemEvent OnUse;
    public ItemEvent OnDrop;
    public Item(){}
    public override bool Equals(object obj){
        Type t = obj.GetType();
        if(t == typeof(Item)){
            return itemName == (obj as Item).itemName;
        }else if(t == typeof(string)){
            return itemName == obj as string;
        }else{
            return false;
        }
    }
    /// <summary>
    /// 调用自定义简易实现方法，不需要生成effect
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="entity"></param>
    /// <param name="args"></param>
    public void InvokeEvent(string eventName, Entity entity = null, string[] args = null){
        GetType().GetMethod(eventName).Invoke(this, new object[]{entity, args});
    }
    /// <summary>
    /// 获取当前物品的贴图
    /// </summary>
    /// <returns></returns>
    public Sprite GetSprite(){
        return SpriteManager.GetSpriteWith(spriteUrl);
    }
    
    //调用OnPick时，args已经通过ItemManager.EventMapping方法解析，只需要传入entity
    public void Pick(Entity entity){
        OnPick?.Invoke(entity, this);
    }
    public void Use(Entity entity){
        OnUse?.Invoke(entity, this);
    }
    public void Drop(Entity entity){
        OnDrop?.Invoke(entity, this);
    }

    # region Event
    /*
    entity参数是在调用Pick，Use，Drop时传入的
    args参数是在json文件里定义的
    */
    public void ShowInfo(Entity entity, string[] args){
        Debug.Log(itemInfo);
    }
    public void Log(Entity entity, string[] args){
        if(args.Length >= 1){
            Debug.Log(args[0]);
        }
    }

    # endregion
    public override int GetHashCode(){
        return itemName.GetHashCode();
    }
    public override string ToString(){
        return $"{itemName}: {itemInfo},{tags}";
    }

}
