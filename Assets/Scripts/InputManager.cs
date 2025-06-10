using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//输入管理，在这里实现多类型输入

public class InputManager
{
    /// <summary>
    /// 菜单键
    /// </summary>
    /// <returns></returns>
    public static bool ESC(){
        if(Global.Controller == "mouse_keyboard"){
            if(Input.GetKeyUp(KeyCode.Escape)){
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 移动方向
    /// </summary>
    /// <returns></returns>
    public static Vector2 MoveToward(){
        if(Global.Controller == "mouse_keyboard"){
            //高优先级，键盘控制移动
            if (Input.GetKey(KeyCode.W)
                || Input.GetKey(KeyCode.A)
                || Input.GetKey(KeyCode.S)
                || Input.GetKey(KeyCode.D)){
                    return new Vector2(
                        Input.GetKey(KeyCode.D) ? 1 : 0,
                        Input.GetKey(KeyCode.W) ? 1 : 0
                    ) - new Vector2(
                        Input.GetKey(KeyCode.A) ? 1 : 0,
                        Input.GetKey(KeyCode.S) ? 1 : 0
                    );
            }
            //低优先级：鼠标右键移动
            if(Input.GetMouseButton(1)){
                Player player = GameBase.now != null ? GameBase.now.player : null;
                if(player != null){
                    Vector2 playerPosition = player.transform.position;
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    return mousePosition - playerPosition;
                }
            }
            return Vector2.zero;
        }
        return Vector2.zero;
    }
    /// <summary>
    /// 攻击方向
    /// </summary>
    /// <returns></returns>
    public static Vector2 AttackToward(){
        if(Global.Controller == "mouse_keyboard"){
            //鼠标左键攻击
            if(Input.GetMouseButton(0)){
                Player player = GameBase.now != null ? GameBase.now.player : null;
                if(player != null){
                    Vector2 playerPosition = player.transform.position;
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    return mousePosition - playerPosition;
                }
            }
            return Vector2.zero;
        }
        return Vector2.zero;
    }
    /// <summary>
    /// 确认键
    /// </summary>
    /// <returns></returns>
    public static bool Enter(){
        if(Global.Controller == "mouse_keyboard"){
            if(Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetMouseButtonDown(0)){
                    return true;
            }
            return false;
        }
        return false;
    }
}
