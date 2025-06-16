using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包类
/// </summary>
public class Bag : MonoBehaviour
{
    public Entity entity;
    public SkipList<string, int> itemList;
    public string activeItem;

    public string ItemInfo{
        get{
            return $"Itemlist:{Tool.GetInfo(itemList)}\nActiveItem:{activeItem}";
        }
    }
    
    void Awake(){
        itemList = new();
        itemList.Compare = (a,b)=>{
            return string.Compare(a.Key,b.Key) >= 0;
        };
        itemList.Equal = (a,b)=>{
            return a.Key == b.Key;
        };

        activeItem = null;
    }
    /// <summary>
    /// 添加物体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    public void AddItem(string itemName){
        var it = itemList.Find(itemName);
        if(it.Key != null){
            itemList.Add(itemName, it.Value + 1, false, true);
        }else{
            itemList.Add(itemName, 1, false);
        }
    }
    /// <summary>
    /// 删除物体
    /// </summary>
    /// <param name="item"></param>
    /// <param name="count"></param>
    public void RemoveItem(string itemName, int count = 1){
        var it = itemList.Find(itemName);
        if(it.Key != null){
            it = new KeyValuePair<string, int>(itemName, it.Value - count);
            if(it.Value <= 0){
                itemList.Remove(itemName);
            }
        }
    }
    /// <summary>
    /// 获取物品对象（根据输入格式自动判断调用方式）
    /// </summary>
    /// <param name="itemIdentifier">物品标识符（格式：dbname.itemname 或 itemname）</param>
    /// <returns>物品对象</returns>
    public Item GetItem(string itemIdentifier){
        if (string.IsNullOrEmpty(itemIdentifier))
            return null;
        // 检查是否为 dbname.itemname 格式
        if (itemIdentifier.Contains(".")){
            var parts = itemIdentifier.Split('.');
            if (parts.Length == 2){
                return ItemManager.GetItem(parts[0], parts[1]);
            }
        }
        // 默认调用全局查找
        return ItemManager.GetItemInAllDB(itemIdentifier);
    }

}
