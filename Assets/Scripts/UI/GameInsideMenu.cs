using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInsideMenu : MonoBehaviour
{   
    public GameObject menu;
    public bool isOpen = false;

    public void OpenMenu(){
        GameManager.Pause();
        Debug.Log("InsideUI: 打开菜单");
        isOpen = true;
        menu.SetActive(true);
    }
    public void BackToGame(){
        GameManager.Pause();
        Debug.Log("InsideUI: 返回游戏");
        isOpen = false;
        menu.SetActive(false);
    }
    public void BackToMainMenu(){
        BackToGame();
        Debug.Log("InsideUI: 返回主菜单");
        Global.ChangeScene("MainMenu");
    }
}
