using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Timer : MonoBehaviour
{   
    /// <summary>
    /// 计时器记录的最大时间，默认为30天的秒数
    /// </summary>
    public const float MAX_TIME = 2628000f;
    protected Coroutine process;
    protected List<float> timer = new List<float>();
    protected List<float> timeTarget = new List<float>();
    protected Dictionary<string, int> timerNames = new Dictionary<string, int>();
    protected List<UnityEvent<float>> events = new List<UnityEvent<float>>();
    /// <summary>
    /// 添加计时器事件
    /// </summary>
    /// <param name="name">计时器名称</param>
    /// <param name="time">计时器时间</param>
    /// <param name="action">计时器事件行为</param>
    /// <param name="replace">是否替换已有事件，默认为否</param>
    public void AddTimer(string name, float time, UnityAction<float> action, bool loop = true, bool replace = false){
        int index = 0;
        if(timerNames.ContainsKey(name)){
            index = timerNames[name];
            timer[index] = 0;
            timeTarget[index] = time;
        }else{
            timer.Add(0);
            timeTarget.Add(time);
            timerNames.Add(name, timer.Count - 1);
            events.Add(new UnityEvent<float>());
        }
        if(replace){
            events[index].RemoveAllListeners();
        }
        events[index].AddListener(action);
        if(!loop){
            events[index].AddListener((t) => {
                events[index].RemoveListener(action); 
            });
        }
    }
    /// <summary>
    /// 改变计时器时长
    /// </summary>
    /// <param name="name"></param>
    /// <param name="time"></param>
    public void ChangeTimer(string name, float time){
        timeTarget[timerNames[name]] = time;
    }
    /// <summary>
    /// 获取计时器状态
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public float GetTimer(string name){
        return timer[timerNames[name]];
    }
    /// <summary>
    /// 清空计时器计数
    /// </summary>
    /// <param name="name"></param>
    public void ClearTimer(string name){
        timer[timerNames[name]] = 0;
    }
    /// <summary>
    /// 清空所有计时器计数
    /// </summary>
    public void ClearAll(){
        timer.Clear();
    }
    /// <summary>
    /// 移除计时器
    /// </summary>
    /// <param name="name"></param>
    public void RemoveTimer(string name){
        timer.RemoveAt(timerNames[name]);
        timeTarget.RemoveAt(timerNames[name]);
        events.RemoveAt(timerNames[name]);
        timerNames.Remove(name);
    }

    void Update(){
        float deltaTime = Time.deltaTime;
        for(int i = 0; i<timer.Count; i++){
            timer[i] += deltaTime;
            if(timer[i] >= timeTarget[i]){
                events[i]?.Invoke(timer[i]);
                timer[i] = 0;
            }
            if(timer[i] >= MAX_TIME){
                timer[i] = 0;
            }
        }
    }
}
