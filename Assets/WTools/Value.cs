using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ValueAffect{
    public string name;
    public float value;
    public static ValueAffect NULL = new ValueAffect();
    public bool isNull{
        get{
            if(name == null){
                return true;
            }else{
                return false;
            }
        }
    }
    public ValueAffect(string name,float value){
        this.name = name;
        this.value = value;
    }
    public ValueAffect(ValueAffect va){
        this.name = va.name;
        this.value = va.value;
    }
    public ValueAffect(ValueAffect va,float value){
        this.name = va.name;
        this.value = value;
    }
    public ValueAffect(string name){
        this.name = name;
        this.value = 0;
    }
    public override bool Equals(object obj){
        return ((ValueAffect)obj).name.Equals(name) && ((ValueAffect)obj).value.Equals(value) ? true : false;
    }
    public override int GetHashCode(){
        return base.GetHashCode();
    }
    public override string ToString(){
        return $"{name}:{value}";
    }
}

[Serializable]
public class ValueAffectList{
    //跳表实现
    [SerializeField]
    protected SkipList<ValueAffect> _list;
    public ValueAffectList(){
        _list = new SkipList<ValueAffect>();
        _list.Compare = (x,y)=>{
            if(x.isNull || y.isNull){
                return false;
            }
            return string.Compare(x.name, y.name) <= 0;
        };
        _list.Equal = (x,y)=>{
            if(x.isNull || y.isNull){
                return false;
            }
            return x.name.Equals(y.name);
        };
    }
    public float count{
        get{
            return _list.count;
        }
    }
    public ValueAffect Find(string name){
        ValueAffect temp = new ValueAffect(name);
        var node = _list.FindNode(temp);
        temp = node == null? ValueAffect.NULL : node.value;
        if(temp.isNull){
            return ValueAffect.NULL;
        }else{
            return temp;
        }
    }
    public float Get(string name){
        ValueAffect va = Find(name);
        return va.value;
    }
    public bool Contains(string name){
        return Find(name).isNull;
    }
    public void Remove(string name){//移除所有同名影响值
        ValueAffect temp = new ValueAffect(name);
        _list.Remove(temp);
    }

    /* 顺序查询的实现
    [SerializeField]
    protected List<ValueAffect> _list;//内存
    public ValueAffectList(){//构造
        _list = new List<ValueAffect>();
    }
    public float count{
        get{
            return _list.Count;
        }
    }

    public ValueAffect Find(string name){//遍历查询
        ValueAffect res = _list.Find(x=>x.name == name);
        if(res.isNull){
            return ValueAffect.NULL;
        }else{
            return res;
        }
    }
    public float Get(string name){
        ValueAffect va = Find(name);
        return va.value;
    }
    public bool Contains(string name){
        return Find(name).isNull;
    }

    public void Remove(string name){//移除所有同名影响值
        if(Contains(name)){
            _list.RemoveAll(x=>x.name == name);
        }
    }
    public void RemoveFirst(string name){//删除第一个同名的影响值
        for(int i = 0; i<_list.Count; i++){
            if(_list[i].name == name){
                _list.RemoveAt(i);
                return;
            }
        }
    }
    public void RemoveLast(string name){//删除最后一个同名的影响值
        for(int i = _list.Count-1; i>=0; i--){
            if(_list[i].name == name){
                _list.RemoveAt(i);
                return;
            }
        }
    }//*/

    public void Set(ValueAffect va, bool replace){//设置影响值
        if(va.isNull)return;
        if(replace){
            _list.Remove(va);
        }
        _list.Add(va);
    }
    public void Set(string name,float value, bool replace){
        Set(new ValueAffect(name,value),replace);
    }

    //Set是新增，存在则删除已有，Add是增加，无视是否存在
    public void Set(ValueAffect va){ 
        Set(va,true);
    }
    public void Set(string name,float value){ 
        Set(new ValueAffect(name,value),true);
    }
    public void Add(ValueAffect va){
        Set(va,false);   
    }
    public void Add(string name,float value){
        Set(new ValueAffect(name,value),false);
    }

    public float GetAll(){
        float res = 0;
        _list.ForEach(va => res += va.value.value);
        return res;
    }//*/

    //object方法重载
    public override string ToString(){
        string res = "";
        _list.ForEach(va => res += va.value.ToString()+" ");
        res += "\n";
        return res;
    }
}
[Serializable]
public class ValueAffectLayer{
    public ValueAffectList affectList;
    public bool isMul = false;
    public string name;
    public ValueAffectLayer(string name,bool isMul){
        this.name = name;
        this.isMul = isMul;
        affectList = new ValueAffectList();
    }
    public override string ToString(){
        return $"{name}:{(isMul? "Mul" : "Sum")}:(\n{affectList.ToString()})";
    }
}

/*
Value类,用于RPG数值的存储
实现了数值的影响值计算，存储了基础值和不同影响层的影响值
在获取数值时会自动计算所有影响值造成的影响
通过增加/删除影响值来改变最终的数值
*/

[Serializable]
public class Value 
{   
    [SerializeField]
    protected float _baseValue;
    [SerializeField]
    protected List<ValueAffectLayer> _affectList;
    public Value(float baseValue){
        _baseValue = baseValue;
        _affectList = new List<ValueAffectLayer>();
    }
    public Value(){
        _baseValue = 0;
        _affectList = new List<ValueAffectLayer>();
    }
    public float value{
        get{
            float res = _baseValue;
            foreach(ValueAffectLayer v in _affectList){
                if(v.isMul){
                    res *= v.affectList.GetAll();
                }else{
                    res += v.affectList.GetAll();
                }
            }
            return res;
        }
    }
    /// <summary>
    /// 设置基础值
    /// </summary>
    /// <param name="value"></param>
    public void Set(float value){//设置基础值
        _baseValue = value;
    }

    public void AddAffectLayer(string name,bool isMul){//添加影响层
        _affectList.Add(new ValueAffectLayer(name,isMul));
    }
    public void RemoveAffectLayer(string name){//移除影响层
        for(int i = 0; i<_affectList.Count; i++){
            if(_affectList[i].name == name){
                _affectList.RemoveAt(i);
                return;
            }
        }
    }
    public ValueAffectLayer GetAffectLayer(string name){//获取影响层
        for(int i = 0; i<_affectList.Count; i++){
            if(_affectList[i].name == name){
                return _affectList[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 设置影响
    /// </summary>
    public void SetAffect(float value,string layerName, string affectName = "BASE", bool mul = false, bool replace = true){//设置影响值
        ValueAffectLayer v = GetAffectLayer(layerName);
        if(v == null){
            AddAffectLayer(layerName,mul);
            v = GetAffectLayer(layerName);
        }
        v.affectList.Set(affectName,value,replace);
    }
    /// <summary>
    /// 添加影响
    /// </summary>
    public void Add(float value, string layerName, string affectName = "BASE"){
        SetAffect(value, layerName, affectName, false, false);
    }
    public void RemoveAffect(string layerName,string affectName){//移除影响值
        ValueAffectLayer v = GetAffectLayer(layerName);
        if(v != null){
            v.affectList.Remove(affectName);
        }
    }

    //运算符重载

    public static Value operator +(Value v, float f){
        v.SetAffect(f, "SumLayer", "BASE", false, false);
        return v;
    }
    public static Value operator -(Value v, float f){
        v.SetAffect(-f, "SumLayer", "BASE", false, false);
        return v;
    }
    public static Value operator *(Value v, float f){
        v.SetAffect(f, "MulLayer", "BASE", true, false);
        return v;
    }
    public static Value operator /(Value v, float f){
        v.SetAffect(1/f, "MulLayer", "BASE", true, false);
        return v;
    }
    public static bool operator ==(Value v1, Value v2){
        return v1.value == v2.value;
    }
    public static bool operator ==(Value v1, float v2){
        return v1.value == v2;
    }
    public static bool operator !=(Value v1, Value v2){
        return v1.value != v2.value;
    }
    public static bool operator !=(Value v1, float v2){
        return v1.value != v2;
    }
    public static bool operator <(Value v1, Value v2){
        return v1.value < v2.value;
    }
    public static bool operator <(Value v1, float v2){
        return v1.value <= v2;
    }
    public static bool operator >(Value v1, Value v2){
        return v1.value > v2.value;
    }
    public static bool operator >(Value v1, float v2){
        return v1.value > v2;
    }
    public static bool operator <=(Value v1, Value v2){
        return v1.value <= v2.value;
    }
    public static bool operator <=(Value v1, float v2){
        return v1.value <= v2;
    }
    public static bool operator >=(Value v1, Value v2){
        return v1.value >= v2.value;
    }
    public static bool operator >=(Value v1, float v2){
        return v1.value >= v2;
    }
    public static implicit operator float(Value v){//隐式转换
        return v.value;
    }
    public static implicit operator Value(float f){
        return new Value(f);
    }

    public override bool Equals(object obj){
        if(obj == null || !(obj is Value)){
            return false;
        }
        Value v = (Value)obj;
        return value.Equals(v.value);
    }

    public override int GetHashCode(){
        return value.GetHashCode();
    }

    public override string ToString(){
        string res = "";
        res += $"baseValue:{_baseValue}\n";
        foreach(ValueAffectLayer v in _affectList){
            res += v.ToString();
        }
        res += $"value:{value}\n";
        return res;
    }

}
