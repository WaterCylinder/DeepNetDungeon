using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//实体类
[Serializable]
public class EntityTag : Tag{
    public static uint None;
    public static uint Player;
    public static uint Enemy;
    public static uint Bullet;
}
public abstract class Entity : MonoBehaviour
{
    public EntityTag entityTags = new EntityTag();
    /// <summary>
    /// 可扩展参数
    /// </summary>
    public List<float> args;
    /// <summary>
    /// 添加参数并返回该参数的索引
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    public int AddArg(float a){
        args.Add(a);
        return args.Count - 1;
    }

    /// <summary>
    /// 作用的效果集
    /// </summary>
    public LoopList<Effect> effects = new LoopList<Effect>(Global.MAX_EFFECT);
    public Effect AddEffect(string name){
        return Effect.AddEffect(this, name);
    }
    public void AddEffect(Effect effect){
        if(CheckEffect(effect.effectName)){
            if(effect.tags.Check(EffectTag.CanRepeat)){
                effects.Add(effect);
                return;
            }
            if(effect.tags.Check(EffectTag.Replace)){
                RemoveEffect(effect);
                effects.Add(effect);
                return;
            }
        }else{
            effects.Add(effect);
        }
    }
    public void RemoveEffect(Effect effect){
        effects.Remove(effect);
    }
    public bool CheckEffect(string name){
        return effects.Exists(e => e.effectName == name);
    }
    public bool CheckEffect(EffectTag tags){
        return effects.Exists(e => e.tags.Check(tags));
    }
    public bool CheckEffect(Effect effect){
        return CheckEffect(effect.effectName);
    }

    //移动相关
    public float speed = 1f;
    public Vector2 toward = Vector2.zero;
    public float hp = 0;

    //基础属性
    /// <summary>
    /// 最大生命值
    /// </summary>
    public Value maxHp = new Value(100f);
    /// <summary>
    /// 护甲值
    /// </summary>
    public Value armor = new Value(0f);
    /// <summary>
    /// 护盾值
    /// </summary>
    public Value shield = new Value(0f);
    /// <summary>
    /// 减伤值
    /// </summary>
    public Value reduc = new Value(0f);
    /// <summary>
    /// 攻击力
    /// </summary>
    public Value attack = new Value(1f);
    /// <summary>
    /// 攻击速度
    /// </summary>
    public Value attackSpeed = new Value(1f);
    /// <summary>
    /// 暴击率
    /// </summary>
    public Value crit = new Value(0f);
    /// <summary>
    /// 暴击倍率
    /// </summary>
    public Value critRatio = new Value(Global.CRIT_RITIO_BASE);
    /// <summary>
    /// 错误指数
    /// </summary>
    public Value error = new Value(0f); 

    protected Rigidbody2D rb;
    protected Timer timer;

    protected virtual void EntityAwake() { }
    protected virtual void EntityStart() { }
    [HideInInspector]public UnityEvent<Entity> StartEvent;
    protected virtual void EntityUpdate() { }
    [HideInInspector]public UnityEvent<Entity> UpdateEvent;
    protected virtual void EntityOnDamage(Damage damage) { }
    [HideInInspector]public UnityEvent<Entity, Damage> DamageEvent;
    protected virtual void EntityOnDead() { }
    [HideInInspector]public UnityEvent<Entity> DeadEvent;
    protected virtual void EntityCollisionEnter(Entity other) { }
    protected virtual void OtherCollisionEnter(GameObject other) { }
    [HideInInspector]public UnityEvent<Entity, Collision2D> CollisionEnterEvent;
    [HideInInspector]public UnityEvent<Entity, Collider2D> TriggerEnterEvent;
    protected virtual void EntityCollisionStay(Entity other) { }
    protected virtual void OtherCollisionStay(GameObject other) { }
    [HideInInspector]public UnityEvent<Entity, Collision2D> CollisionStayEvent;
    [HideInInspector]public UnityEvent<Entity, Collider2D> TriggerStayEvent;
    protected virtual void EntityCollisionExit(Entity other) { }
    protected virtual void OtherCollisionExit(GameObject other) { }
    [HideInInspector]public UnityEvent<Entity, Collision2D> CollisionExitEvent;
    [HideInInspector]public UnityEvent<Entity, Collider2D> TriggerExitEvent;
    protected virtual void EntityAttack() { }

    [SerializeField]
    protected bool canAttack = true;

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        timer = gameObject.AddComponent<Timer>();
        EntityAwake();
    }

    void Start(){
        timer.AddTimer("AttackTimer", attackSpeed.value, t=>{
            canAttack = true;
        });
        hp = maxHp.value;
        EntityStart();
        StartEvent?.Invoke(this);
    }

    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.GetComponent<Entity>() != null){
            EntityCollisionEnter(other.gameObject.GetComponent<Entity>());
        }else{
            OtherCollisionEnter(other.gameObject);
        }
        CollisionEnterEvent?.Invoke(this,other);
    }

    void OnCollisionStay2D(Collision2D other){
        if(other.gameObject.GetComponent<Entity>() != null){
            EntityCollisionStay(other.gameObject.GetComponent<Entity>());
        }else{
            OtherCollisionStay(other.gameObject);
        }
        CollisionStayEvent?.Invoke(this,other);
    }

    void OnCollisionExit2D(Collision2D other){
        if(other.gameObject.GetComponent<Entity>() != null){
            EntityCollisionExit(other.gameObject.GetComponent<Entity>());
        }else{
            OtherCollisionExit(other.gameObject);
        }
        CollisionExitEvent?.Invoke(this,other);
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.GetComponent<Entity>() != null){
            EntityCollisionEnter(other.GetComponent<Entity>());
        }else{
            OtherCollisionEnter(other.gameObject);
        }
        TriggerEnterEvent?.Invoke(this,other);
    }

    void OnTriggerStay2D(Collider2D other){
        if(other.GetComponent<Entity>() != null){
            EntityCollisionStay(other.GetComponent<Entity>());
        }else{
            OtherCollisionStay(other.gameObject);
        }
        TriggerStayEvent?.Invoke(this,other);
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.GetComponent<Entity>() != null){
            EntityCollisionExit(other.GetComponent<Entity>());
        }else{
            OtherCollisionExit(other.gameObject);
        }
        TriggerExitEvent?.Invoke(this,other);
    }

    void Update(){
        Move();
        EntityUpdate();
        UpdateEvent?.Invoke(this);
    }

    public void AddDamage(Damage damage){
        Damage d = new Damage(damage);
        d.tags.Add(Tool.RandomPercent(d.crit)? DamageTag.Crit:0);
        EntityOnDamage(d);
        DamageEvent?.Invoke(this, d);
        Debug.Log($"{name} 受到了 {d.source.name} 的攻击 {d.value}\n {d}");
        if(d.tags.Check(DamageTag.Normal)){
            hp -= d.value;
        }
    }

    protected virtual void Move(){
        rb.velocity = toward.normalized * speed;
    }

    protected void MoveStop(){
        toward = Vector2.zero;
    }

    protected void Attack(){
        if(canAttack){
            canAttack = false;
            EntityAttack();
            float at = attackSpeed.value;
            if(at != 0){
                at = 1 / at;
            }
            timer.ChangeTimer("AttackTimer", at);
        }
    }

    /// <summary>
    /// 对象死亡
    /// </summary>
    protected void Dead(){
        EntityOnDead();
        DeadEvent?.Invoke(this);
        Debug.Log($"{name} 死亡");
        GameManager.instance.EntityDestroy(this);
    }
}
