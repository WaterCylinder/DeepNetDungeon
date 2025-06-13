using UnityEngine;
using UnityEngine.Events;
using System;
[Serializable]
public class BulletTag : Tag{
    public static uint None;
    public static uint Player;
    public static uint Enemy;
    /// <summary>
    /// 可穿透实体
    /// </summary>
    public static uint PenetrateEntity;
    /// <summary>
    /// 可穿透方块（障碍物）
    /// </summary>
    public static uint PenetrateBlock;
    /// <summary>
    /// 持续伤害
    /// </summary>
    public static uint DoT;
    /// <summary>
    /// 忽略阵营判断
    /// </summary>
    public static uint IgnoreFaction;
    /// <summary>
    /// 禁用根据toward旋转
    /// </summary>
    public static uint DisableRotateWithToward;
    /// <summary>
    /// 禁用忽略源碰撞
    /// </summary>
    public static uint DisableIgnoreSourceCollision;
    /// <summary>
    /// 禁用忽略子弹间的碰撞
    /// </summary>
    public static uint DisableIgnoreBulletCollision;
}
public class Bullet : Entity
{   
    private static GameObject _bulletPrefab;
    public static GameObject bulletPrefab{
        get{
            if(_bulletPrefab == null){
                _bulletPrefab = AssetManager.LoadEntity("Bullet", false);
            }
            return _bulletPrefab;
        } 
    }
    public static Bullet CreateBullet(){
        GameObject obj = Instantiate(bulletPrefab);
        Bullet bullet = obj.GetComponent<Bullet>();
        GameManager.instance.EntityAdd(bullet);
        return obj.GetComponent<Bullet>();
    }
    /// <summary>
    /// 创建一个可以造成普通伤害的弹幕对象，
    /// </summary>
    /// <param name="source"></param>
    /// <param name="position"></param>
    /// <param name="toward"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static Bullet CreateBullet(Entity source, Vector2 position, Vector2 toward, float speed){
        Bullet but = CreateBullet();
        BulletTag t = new();
        if(source.entityTags.Check(EntityTag.Enemy)){
            t.Add(BulletTag.Enemy);
        }
        if(source.entityTags.Check(EntityTag.Player)){
             t.Add(BulletTag.Player);
        }
        but.BulletInit(
            source,
            bulletTags: t,
            EnterEvent: (but, e)=>{
                but.damage.tags.Add(DamageTag.Normal);
                e.AddDamage(but.damage);
                //没有穿过实体标签的弹幕碰到子弹触发消失
                if(!but.bulletTags.Check(BulletTag.PenetrateEntity)){
                    but.Dead();
                }
            }
        );
        but.transform.position = position;
        but.toward = toward;
        but.speed = speed;
        GameManager.instance.EntityAdd(but);
        return but;
    }
    public BulletTag bulletTags = new BulletTag();
    public Damage damage;
    public Entity source;
    /// <summary>
    /// 弹幕生命周期，小于等于0则持续时间无限
    /// </summary>
    public float life = 0;
    /// <summary>
    /// 初始化弹幕对象
    /// </summary>
    /// <param name="value">伤害值</param>
    /// <param name="crit">继承自来源数值的暴击率</param>
    /// <param name="critRatio">继承自来源数值的暴击伤害倍率</param>
    /// <param name="source">来源实体</param>
    /// <param name="toward">朝向</param>
    /// <param name="speed">速度</param>
    /// <param name="position">初始位置</param>
    /// <param name="rotation">初始朝向</param>
    /// <param name="scale">大小</param>
    /// <param name="life">持续时间</param>
    /// <param name="bulletTags">标签</param>
    /// <param name="StartEvent">生成时事件</param>
    /// <param name="UpdateEvent">每帧事件</param>
    /// <param name="DeadEvent">消失事件</param>
    /// <param name="EnterEvent">检测器进入实体事件</param>
    /// <param name="StayEvent">检测器在实体中事件</param>
    /// <param name="ExitEvent">检测器离开实体事件</param>
    public void BulletInit(float value, float crit, float critRatio, 
        Entity source, Vector2 toward, float speed,
        Vector3 position, Quaternion rotation, Vector3 scale,
        float life = 0,
        uint bulletTags = 0,
        UnityAction<Bullet> StartEvent = null,
        UnityAction<Bullet> UpdateEvent = null,
        UnityAction<Bullet> DeadEvent = null,
        UnityAction<Bullet, Entity> EnterEvent = null,
        UnityAction<Bullet, Entity> StayEvent = null,
        UnityAction<Bullet, Entity> ExitEvent = null
    ){
        Damage damage = new Damage(value, crit, critRatio);
        this.damage = damage;
        this.attack.Set(value);
        this.crit.Set(crit);
        this.critRatio.Set(critRatio);
        this.source = source;
        if(source != null){
            damage.source = source;
            // 剔除与弹幕来源的碰撞检测
            if(!this.bulletTags.Check(BulletTag.DisableIgnoreSourceCollision))
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), source.GetComponent<Collider2D>());
        }
        this.toward = toward;
        this.speed = speed;
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
        this.life = life;
        this.bulletTags.Add(bulletTags);
        if(StartEvent != null)this.StartEvent.AddListener((e) => {StartEvent?.Invoke((Bullet)e);});
        if(UpdateEvent != null)this.UpdateEvent.AddListener((e) => {UpdateEvent?.Invoke((Bullet)e);});
        if(DeadEvent != null)this.DeadEvent.AddListener((e) => {DeadEvent?.Invoke((Bullet)e);});
        if(EnterEvent != null)
            TriggerEnterEvent.AddListener((e, o) => {
                if(o.GetComponent<Entity>() != null){
                    EntityTag et = o.GetComponent<Entity>().entityTags;
                    BulletTag bt = this.bulletTags;

                    if(!(!bt.Check(BulletTag.DisableIgnoreBulletCollision) && et.Check(EntityTag.Bullet))
                        && !(bt.Check(BulletTag.IgnoreFaction)
                            && ((et.Check(EntityTag.Enemy) && bt.Check(BulletTag.Enemy)) 
                                || (et.Check(EntityTag.Player) && bt.Check(BulletTag.Player))
                                )
                            )
                    )EnterEvent?.Invoke((Bullet)e, o.GetComponent<Entity>());
                }
            });
        if(StayEvent != null)
            TriggerStayEvent.AddListener((e, o) => {
                if(o.GetComponent<Entity>() != null){
                    EntityTag et = o.GetComponent<Entity>().entityTags;
                    BulletTag bt = this.bulletTags;

                    if(!(!bt.Check(BulletTag.DisableIgnoreBulletCollision) && et.Check(EntityTag.Bullet))
                        && !(!bt.Check(BulletTag.IgnoreFaction)
                            && ((et.Check(EntityTag.Enemy) && bt.Check(BulletTag.Enemy)) 
                                || (et.Check(EntityTag.Player) && bt.Check(BulletTag.Player))
                                )
                            )
                    )StayEvent?.Invoke((Bullet)e, o.GetComponent<Entity>());
                }
            });
        if(ExitEvent != null)
            TriggerExitEvent.AddListener((e, o) => {
                if(o.GetComponent<Entity>() != null){
                    EntityTag et = o.GetComponent<Entity>().entityTags;
                    BulletTag bt = this.bulletTags;

                    if(!(!bt.Check(BulletTag.DisableIgnoreBulletCollision) && et.Check(EntityTag.Bullet))
                        && !(bt.Check(BulletTag.IgnoreFaction)
                            && ((et.Check(EntityTag.Enemy) && bt.Check(BulletTag.Enemy)) 
                                || (et.Check(EntityTag.Player) && bt.Check(BulletTag.Player))
                                )
                            )
                    )ExitEvent?.Invoke((Bullet)e, o.GetComponent<Entity>());
                }
            });
    }
    /// <summary>
    /// 初始化弹幕对象
    /// </summary>
    /// <param name="damage">伤害对象</param>
    /// <param name="source">来源实体</param>
    /// <param name="toward">朝向</param>
    /// <param name="speed">速度</param>
    /// <param name="position">初始位置</param>
    /// <param name="rotation">初始朝向</param>
    /// <param name="scale">大小</param>
    /// <param name="life">持续时间</param>
    /// <param name="bulletTags">标签</param>
    /// <param name="StartEvent">生成时事件</param>
    /// <param name="UpdateEvent">每帧事件</param>
    /// <param name="DeadEvent">消失事件</param>
    /// <param name="EnterEvent">检测器进入实体事件</param>
    /// <param name="StayEvent">检测器在实体中事件</param>
    /// <param name="ExitEvent">检测器离开实体事件</param>
    public void BulletInit(Damage damage,
        Entity source, Vector2 toward, float speed,
        Vector3 position, Quaternion rotation, Vector3 scale,
        float life = 0,
        uint bulletTags = 0,
        UnityAction<Bullet> StartEvent = null,
        UnityAction<Bullet> UpdateEvent = null,
        UnityAction<Bullet> DeadEvent = null,
        UnityAction<Bullet, Entity> EnterEvent = null,
        UnityAction<Bullet, Entity> StayEvent = null,
        UnityAction<Bullet, Entity> ExitEvent = null
    ){
        BulletInit(
            damage.value, damage.crit, damage.critRatio,
            source, toward, speed,
            position, rotation, scale,
            life,
            bulletTags,
            StartEvent,
            UpdateEvent,
            DeadEvent,
            EnterEvent,
            StayEvent,
            ExitEvent
        );
    }
    /// <summary>
    /// 初始化弹幕对象
    /// </summary>
    /// <param name="damage">伤害对象</param>
    /// <param name="source">来源实体</param>
    /// <param name="life">持续时间</param>
    /// <param name="StartEvent">生成时事件</param>
    /// <param name="UpdateEvent">每帧事件</param>
    /// <param name="DeadEvent">消失事件</param>
    /// <param name="EnterEvent">检测器进入实体事件</param>
    /// <param name="StayEvent">检测器在实体中事件</param>
    /// <param name="ExitEvent">检测器离开实体事件</param>
    public void BulletInit(Damage damage, Entity source, float life = 0, uint bulletTags = 0,
        UnityAction<Bullet> StartEvent = null,
        UnityAction<Bullet> UpdateEvent = null,
        UnityAction<Bullet> DeadEvent = null,
        UnityAction<Bullet, Entity> EnterEvent = null,
        UnityAction<Bullet, Entity> StayEvent = null,
        UnityAction<Bullet, Entity> ExitEvent = null
    ){
        BulletInit(damage, source, toward,
            speed, transform.position, default, Vector3.one,
            life,
            bulletTags,
            StartEvent,
            UpdateEvent,
            DeadEvent,
            EnterEvent,
            StayEvent,
            ExitEvent
        );
    }
    /// <summary>
    /// 初始化弹幕对象
    /// </summary>
    /// <param name="source">来源实体</param>
    /// <param name="life">持续时间</param>
    /// <param name="StartEvent">生成时事件</param>
    /// <param name="UpdateEvent">每帧事件</param>
    /// <param name="DeadEvent">消失事件</param>
    /// <param name="EnterEvent">检测器进入实体事件</param>
    /// <param name="StayEvent">检测器在实体中事件</param>
    /// <param name="ExitEvent">检测器离开实体事件</param>
    public void BulletInit(Entity source, 
        float life = 0,
        uint bulletTags = 0,
        UnityAction<Bullet> StartEvent = null,
        UnityAction<Bullet> UpdateEvent = null,
        UnityAction<Bullet> DeadEvent = null,
        UnityAction<Bullet, Entity> EnterEvent = null,
        UnityAction<Bullet, Entity> StayEvent = null,
        UnityAction<Bullet, Entity> ExitEvent = null
    ){  
        Damage damage = new Damage(0); 
        if(source != null)
            damage = new Damage(source.attack.value, source.crit.value, source.critRatio.value);
        BulletInit(
            damage,
            source,
            life,
            bulletTags,
            StartEvent,
            UpdateEvent,
            DeadEvent,
            EnterEvent,
            StayEvent,
            ExitEvent
        );
    }
    /// <summary>
    /// 设置事件
    /// </summary>
    /// <param name="StartEvent"></param>
    /// <param name="UpdateEvent"></param>
    /// <param name="DeadEvent"></param>
    /// <param name="EnterEvent"></param>
    /// <param name="StayEvent"></param>
    /// <param name="ExitEvent"></param>
    public void SetEvent(
        UnityAction<Bullet> StartEvent = null,
        UnityAction<Bullet> UpdateEvent = null,
        UnityAction<Bullet> DeadEvent = null,
        UnityAction<Bullet, Entity> EnterEvent = null,
        UnityAction<Bullet, Entity> StayEvent = null,
        UnityAction<Bullet, Entity> ExitEvent = null
    ){
        BulletInit(
            source,
            life,
            bulletTags,
            StartEvent,
            UpdateEvent,
            DeadEvent,
            EnterEvent,
            StayEvent,
            ExitEvent
        );
    }
    protected override void EntityStart(){
        entityTags.Add(EntityTag.Bullet);
        if(life > 0){
            timer.AddTimer("BulletLifeTimer", life, t=>{
                Dead();
            });
        }
    }

    protected override void EntityUpdate(){
        //如果没有禁用旋转，则根据射出角度改变旋转角度
        if(!bulletTags.Check(BulletTag.DisableRotateWithToward)){
            transform.localEulerAngles = Tool.RightAngleRotate(toward);
        }
    }

}
