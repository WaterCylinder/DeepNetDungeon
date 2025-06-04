/*
普通伤害Normal：没有特殊效果的伤害。
特殊伤害Super：无视护甲值的伤害。
护甲伤害Armor：对护甲值造成损伤，不同于效果造成的护甲值减少，这将直接永久降低基础护甲值。
护盾伤害ShieldOnly：只对护甲造成伤害。
穿甲伤害ShieldCross：无视护盾值的伤害。
真实伤害True：无视护甲值和减伤倍率的伤害。
错误伤害Error：根据错误指数浮动伤害。
暴击伤害Crit：计算过暴击倍率的暴击伤害。
弹幕伤害Bullet：弹幕伤害。
接触伤害Touch：实体接触伤害。
*/
using System;
using UnityEngine;
[Serializable]
public class DamageTag : Tag{
    public static uint None;
    public static uint Normal;
    public static uint Super;
    public static uint Armor;
    public static uint ShieldOnly;
    public static uint ShieldCross;
    public static uint True;
    public static uint Error;
    public static uint Crit;
    public static uint Bullet;
    public static uint Touch;
}
[Serializable]
public class Damage{
    [SerializeField]
    public DamageTag tags = new DamageTag();
    /// <summary>
    /// 伤害值
    /// </summary>
    public float _value = 0;
    public float value{
        get{
            return tags.Check(DamageTag.Crit) ? _value * critRatio : _value;
        }
    }
    /// <summary>
    /// 暴击率
    /// </summary>
    public float crit = 0;
    /// <summary>
    /// 暴击倍率
    /// </summary>
    public float critRatio = Global.CRIT_RITIO_BASE;
    /// <summary>
    /// 来源实体
    /// </summary>
    public Entity source;
    public Damage(float _value,float crit,float critRatio){
        this._value = _value;
        this.crit = crit;
        this.critRatio = critRatio;
    }
    public Damage(float _value){
        this._value = _value;
    }
    public Damage(Damage damage){
        this._value = damage._value;
        this.crit = damage.crit;
        this.critRatio = damage.critRatio;
        this.source = damage.source;
        this.tags.Add(damage.tags.value);
    }
    
    override public string ToString(){
        return $"Damage base:{_value}[\nIsCrit?:{tags.Check(DamageTag.Crit)};\nCritRate:{crit};\nCritRatio:{critRatio};\n]value:{value}";
    }
}
