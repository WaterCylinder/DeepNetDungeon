using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTag : Tag{
    public static uint None = 0;
    /// <summary>
    /// 持续性效果
    /// </summary>
    public static uint Dot;
    /// <summary>
    /// 层数随时间递增
    /// </summary>
    public static uint LayerUp;
    /// <summary>
    /// 层数不随时间变化
    /// </summary>
    public static uint LayerStatic;
    /// <summary>
    /// 层数随时间递减
    /// </summary>
    public static uint LayerDown;
    /// <summary>
    /// 到达层数限制时结束效果
    /// </summary>
    public static uint EndWhenMaxLayer;
    /// <summary>
    /// 同名效果可以重复
    /// </summary>
    public static uint CanRepeat;
    /// <summary>
    /// 会替代同名效果
    /// </summary>
    public static uint Replace;
    /// <summary>
    /// 是否使用效果贴图
    /// </summary>
    public static uint Texture;
}

public abstract class Effect
{   
    public static Effect AddEffect(Entity entity, string name, string[] args = null){
        if(entity == null){
            Debug.LogError($"{entity} 实体不存在");
            return null;
        }
        Type e = Global.Effects.GetType(name);
        if(e == null){
            Debug.LogError($"找不到效果{name}");
            return null;
        }
        return (Effect)Activator.CreateInstance(e, new object[]{ entity, args});
    }
    public string effectName = "ERROR:name set error";
    public readonly Coroutine process;
    public string[] args;
    public Entity entity;
    public EffectTag tags;
    public float processTime;
    public float timer;
    public float layerTime;
    public int defaultLayer;
    public int maxLayer;
    public int layer;
    public Sprite texture;
    public Effect(Entity entity, string[] args = null){
        if(entity == null)
            return;
        effectName = GetType().Name;
        tags = new EffectTag();
        //默认效果为降低层数只有一层且层数结束时消失的持续效果
        tags.Add(EffectTag.Dot | EffectTag.LayerDown | EffectTag.EndWhenMaxLayer);
        float processTime = 1;
        int defaultLayer = 1; 
        float layerTime = 1;
        int maxLayer = 1;

        Init(ref tags, ref processTime, ref defaultLayer, ref layerTime, ref maxLayer);
        this.defaultLayer = defaultLayer;
        this.layerTime = layerTime;
        this.maxLayer = maxLayer;
        
        this.args = args;
        OnStart();
        Reset(entity, processTime);
        process = entity.StartCoroutine(MainProcess());
        entity.AddEffect(this);
    }

    protected abstract void Init(ref EffectTag tags, ref float processTime, ref int defaultLayer, ref float layerTime, ref int maxLayer);
    protected virtual void OnStart() { }
    /// <summary>
    /// 重新设置时执行，创建效果会执行一次，在Start后面执行
    /// </summary>
    protected virtual void OnReset() { }
    /// <summary>
    /// 协程处理时执行
    /// </summary>
    protected virtual void OnProcess() { }
    /// <summary>
    /// 每帧执行
    /// </summary>
    protected virtual void OnUpdate() { }
    /// <summary>
    /// 层数增长时执行
    /// </summary>
    protected virtual void OnLayerUp() { }
    /// <summary>
    /// 仅在层数发生变化时执行
    /// </summary>
    protected virtual void OnLayerChange() { }
    /// <summary>
    /// 层数减少时执行
    /// </summary>
    protected virtual void OnLayerDown() { }
    /// <summary>
    /// 结束时执行
    /// </summary>
    protected virtual void OnEnd() { }
    
    public void LayerUp(){
        if(!tags.Check(EffectTag.LayerStatic)){
            if(layer < maxLayer)layer ++;
            OnLayerUp();
        }
    }
    public void LayerDown(){
        if(!tags.Check(EffectTag.LayerStatic)){
            if(layer > 0)layer --;
            OnLayerDown();
        }
    }

    private IEnumerator MainProcess(){
        while(true){
            //非持续性效果执行一次就结束
            try{
                if(!tags.Check(EffectTag.Dot)){
                    OnProcess();
                    End();
                    yield break;
                }
            }catch(Exception e){
                Debug.Log($"{effectName}已经销毁,{e.Message}");
                End();
            }
            yield return new WaitForSeconds(processTime);
            try{
                timer += processTime;
                OnProcess();
                if(layerTime > 0 && timer >= layerTime){
                    if(tags.Check(EffectTag.LayerUp)){
                        LayerUp();
                    }
                    if(tags.Check(EffectTag.LayerDown)){
                        LayerDown();
                    }
                    OnLayerChange();
                    if(tags.Check(EffectTag.EndWhenMaxLayer)){
                        if(tags.Check(EffectTag.LayerUp) && layer > maxLayer){
                            End();
                        }
                        if(tags.Check(EffectTag.LayerDown) && layer <= 0){
                            End();
                        }
                    }
                    timer = 0;
                }
            }catch(Exception e){
                Debug.Log($"{effectName}已经销毁,{e.Message}");
                End();
                yield break;
            }
            
        }
    }
    public void Reset(Entity entity = null, float processTime = -1){
        timer = 0;
        layer = this.defaultLayer;
        this.entity = entity == null ? this.entity : entity;
        this.processTime = processTime < 0 ? this.processTime : processTime;
        OnReset();
    }
    public void End(){
        entity?.StopCoroutine(process);
        entity?.RemoveEffect(this);
        OnEnd();
        entity = null;
        tags = null;
    }

    public override string ToString(){
        return $"{effectName}: [\nentity:{entity}\ntags:{tags}\nprocessTime:{processTime}\nlayerTime:{defaultLayer}"+
        $"\ntimer:{timer}\ndefaultLayer:{defaultLayer}\nmaxLayer:{maxLayer}\nlayer:{layer}\n]";
    }
}
