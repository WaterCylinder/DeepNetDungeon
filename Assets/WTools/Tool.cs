using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEngine;

public class Tool
{
    public static XDocument ReadXml(string path){
        Debug.Log($"正在读取XML文件：{path}");
        string p = Path.GetDirectoryName(path);
        if(!Directory.Exists(p)){
            Directory.CreateDirectory(p);
            Debug.Log($"XML文件路径不存在，已创建文件夹：{p}");
        }
        if(!File.Exists(path)){
            Debug.Log($"XML文件不存在，请手动创建文件：{path}");
            return null;
        }
        return XDocument.Load(path);
    }
    /// <summary>
    /// 向量与右向量的夹角，带正负
    /// </summary>
    /// <param name="toward"></param>
    /// <returns></returns>
    public static float RightAngle(Vector2 toward){
        return -Mathf.Atan2(toward.x, toward.y) * Mathf.Rad2Deg + 90;
    }
    public static Vector3 RightAngleRotate(Vector2 toward){
        return new Vector3(0,0,RightAngle(toward));
    }
    public static float RandomRange(float min, float max){
        return UnityEngine.Random.Range(min, max);
    }
    public static int RandomRange(int min, int max){
        return UnityEngine.Random.Range(min, max);
    }
    public static bool RandomPercent(int per){
        return UnityEngine.Random.Range(0,100) < per;
    }
    public static bool RandomPercent(float per){
        return UnityEngine.Random.Range(0f,1f) < per;
    }
    /// <summary>
    /// 字符串转换成路径
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToPath(string str){
        if(str.Contains(":") || str==null){
            return str;
        }
        str.Replace("\\", "/").Replace("_", "/").Replace(".","/");
        return Path.Combine(str.Split('/'));
    }
    /// <summary>
    /// 获取迭代对象的信息
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetInfo(IEnumerable list){
        StringBuilder info = new StringBuilder("[");
        IEnumerator iem = list.GetEnumerator();
        while(iem.MoveNext()){
            info.Append(iem.Current.ToString() + ", ");
        }
        return info + "]";
    }
    /// <summary>
    /// 二分查找
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="target"></param>
    /// <param name="compare"></param>
    /// <returns></returns>
    public static T BinaryFind<T>(List<T> list, T target, Func<T, T, int> compare){
        try{
            int l = 0, r = list.Count - 1;
            while(l <= r){
                int mid = l + (r - l) / 2;
                int compResult = compare.Invoke(list[mid], target);
                
                if(compResult == 0){
                    return list[mid];
                }else if(compResult < 0){
                    l = mid + 1;
                }else{
                    r = mid - 1;
                }
            }
            return default;
        }catch (Exception e){ 
            Debug.LogWarning(e.Message);
            return default;
        }
    }
    /// <summary>
    /// 加载sprite从png文件里
    /// </summary>
    /// <param name="path"></param>
    /// <param name="filterMode"></param>
    /// <param name="pixelsPerUnit"></param>
    /// <returns></returns>
    public static Sprite LoadSpriteFromeFile(string path, FilterMode filterMode = FilterMode.Bilinear, float pixelsPerUnit = 100f){
        try{
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            if (!tex.LoadImage(fileData)){ // 关键：实际加载图像数据
                Debug.LogWarning("图像加载失败: " + path);
                return null;
            }
            tex.filterMode = filterMode;
            Sprite sprite = Sprite.Create(
                tex,                      // 基础纹理
                new Rect(0, 0, tex.width, tex.height),  // 裁剪矩形
                new Vector2(0.5f, 0.5f),  // 轴心点(Pivot)
                pixelsPerUnit            // pixelsPerUnit
            );
            return sprite;
        }catch (Exception e){ 
            Debug.LogWarning(e.Message);
            return null;
        }
    }
    /// <summary>
    /// 随机权重抽取
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="rand"></param>
    /// <returns></returns>
    public static T WeightRandomPick<T>(List<WeightData<T>> list, System.Random rand = null){
        int W = 0;
        list.ForEach(x => W += x.weight);
        int r = rand == null ? RandomRange(0, W) : rand.Next(0, W);
        foreach(var x in list){
            W -= x.weight;
            if(r > W){
                return x.data;
            }
        }
        return default;
    }
    public static T WeightRandomPick<T>(WeightData<T>[] list, System.Random rand = null){
        return WeightRandomPick(new List<WeightData<T>>(list), rand);
    }
}
