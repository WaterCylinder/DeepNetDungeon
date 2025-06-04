using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
实体行为脚本的热更新代理
热更新实体需要挂载此脚本以加载热更新实体
*/

public class EntityBehaviorLoader : MonoBehaviour
{
    public string behaviorName = "None";
    //基础属性
    // 最大生命值
    public float maxHp = 100f;
    // 护甲值
    public float armor = 0f;
    // 护盾值
    public float shield = 0f;
    // 减伤值
    public float reduc = 0f;
    // 攻击力
    public float attack = 1f;
    // 攻击速度
    public float attackSpeed = 1f;
    // 暴击率
    public float crit = 0f;
    // 暴击倍率
    public float critRatio = Global.CRIT_RITIO_BASE;
    // 错误指数
    public float error = 0f; 
    void Awake(){
        if(behaviorName != "None"){
            Entity e = gameObject.AddComponent(Global.EntityBehaviors.GetType(behaviorName)) as Entity;
            e.maxHp = maxHp;
            e.armor = armor;
            e.shield = shield;
            e.reduc = reduc;
            e.attack = attack;
            e.attackSpeed = attackSpeed;
            e.crit = crit;
            e.critRatio = critRatio;
            e.error = error;
            Destroy(this);
        }
    }
}
