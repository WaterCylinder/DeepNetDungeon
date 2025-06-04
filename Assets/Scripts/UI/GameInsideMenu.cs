using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInsideMenu : MonoBehaviour
{   
    public GameObject menu;
    public bool isOpen = false;

    public void OpenMenu(){
        GameManager.Pause();
        isOpen = true;
        menu.SetActive(true);
    }
    public void BackToGame(){
        GameManager.Pause();
        isOpen = false;
        menu.SetActive(false);
    }
    public void BackToMainMenu(){
        BackToGame();
        Global.ChangeScene("MainMenu");
    }
}
