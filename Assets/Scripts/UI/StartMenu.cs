using UnityEngine;

public class StartMenu : MonoBehaviour
{
    void Start(){
        GameManager.instance.Init();
    }
    public void StartGame(){
        Global.ChangeScene("MainScene");
    }

    public void Setting(){
        Global.ChangeScene("SettingScene");
    }

    public void QuitGame(){
        Application.Quit();
    }
}
