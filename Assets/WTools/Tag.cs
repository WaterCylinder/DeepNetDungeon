using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
/*
Tag类的使用：
定义TempTag类继承自Tag，在TempTag中定义public static uint的变量，实现类枚举的效果。
标记子类为可实例化以实现在Unity中的自动属性显示。
例如：
[Serializable] public class TempTag : Tag
{
    public static uint None;
    public static uint A ;
    public static uint B ;
    public static uint C ;
}
TempTag t = new TempTag();//会自动将符合要求的变量进行赋值，每个类只赋值一次。
t.Add(TagTemp.A);//添加tag，类似于枚举的用法
t.Check(TagTemp.A | TagTemp.B);//检查tag,
*/
[Serializable]
public class Tag
{   
    private static List<Type> typeInit = new List<Type>();
    private static Dictionary<Type, Dictionary<string, uint>> typeTagInfos = new();
    private uint _value;
    public uint value{
        get{
            return _value;
        }
    }
    public Tag(){
        Type t = GetType();
        if(typeInit.Contains(t)){
            _value = 0;
            return;
        }
        int bitIndex = 0;
        typeInit.Add(t);
        typeTagInfos.Add(t, new());
        foreach(var field in GetType().GetFields(BindingFlags.Public | BindingFlags.Static)){
            if(field.FieldType == typeof(uint)){
                uint bit = 1u << bitIndex;
                field.SetValue(this, bit);
                typeTagInfos[t].Add(field.Name, bit);
                bitIndex++;
            }
        }
        _value = 0;
    }
    public void Add(uint tag){
        _value |= tag;
    }
    public void Remove(uint tag){
        _value &= ~tag;
    }
    public bool Check(uint tag){
        return (_value & tag) != 0;
    }
    public bool CheckAnd(uint tag1, uint tag2){
        return Check(tag1) && Check(tag2);
    }
    public bool CheckAnd(uint tag1, uint tag2, uint tag3){
        return Check(tag1) && Check(tag2) && Check(tag3);
    }
    public bool CheckAnd(uint tag1, uint tag2, uint tag3, uint tag4){
        return Check(tag1) && Check(tag2) && Check(tag3) && Check(tag4);
    }
    public bool CheckAnd(uint[] tags){
        return !tags.Any(tag => !Check(tag));
    }
    public bool CheckOr(uint tag1, uint tag2){
        return Check(tag1 | tag2);
    }
    public bool CheckOr(uint tag1, uint tag2, uint tag3){
        return Check(tag1 | tag2 | tag3);
    }
    public bool CheckOr(uint tag1, uint tag2, uint tag3, uint tag4){
        return Check(tag1 | tag2 | tag3 | tag4);
    }
    public bool CheckOr(uint[] tags){
        return tags.Any(tag => Check(tag));
    }

    public uint GetValue(string tagsName){
        string[] names = tagsName.Split('|', StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, uint> dic = typeTagInfos[GetType()];
        uint v = 0;
        foreach(string n in names){
            v |= dic[n];
        }
        return v;
    }

    public static implicit operator uint(Tag tag){
        return tag._value;
    }
    public static implicit operator Tag(uint value){
        Tag tag = new Tag();
        tag._value = value;
        return tag;
    }
    public override string ToString(){
        Type t = GetType();
        string res = "[";
        foreach(var field in t.GetFields(BindingFlags.Static | BindingFlags.Public)){
            if(field.FieldType == typeof(uint)){
                if(Check((uint)field.GetValue(this))){
                    res += $"| {t.Name}.{field.Name} |";
                }
            }
        }
        return res + "]";
    }
}
