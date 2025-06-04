using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//伤害测试人偶

public class EnemyScarecrow : Enemy
{   
    public SmallText smallText;
    protected override void EntityStart(){
        base.EntityStart();
        //每秒输出该秒收到的伤害值
        timer.AddTimer("DPSCheck", 1f, t=>{
            smallText.context = "DPS:"+ (maxHp.value - hp);
            hp = maxHp;
        }, true, true);
    }
}
