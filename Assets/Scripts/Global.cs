using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{   
    # region ConstField
    /// <summary>
    /// 主进程步进时长
    /// </summary>
    public const float MAINPROCESS_TIMESTEP = 0.01f;
    /// <summary>
    /// 游戏进程步进时长
    /// </summary>
    public const float GAMEPROCESS_TIMESTEP = 0.01f;
    /// <summary>
    /// 演出帧数
    /// </summary>
    public const float VIEW_FPS = 30;
    /// <summary>
    /// 最大存档数量
    /// </summary>
    public const int SAVENUM = 3;
    /// <summary>
    /// 最大资源同时加载数
    /// </summary>
    public const int MAX_ASSETLOADER = 32;
    /// <summary>
    /// 基础暴击倍率
    /// </summary>
    public const float CRIT_RITIO_BASE = 1.5f;
    /// <summary>
    /// 护甲基准值
    /// </summary>
    public const float ARMOR_BASEVALUE = 100;
    /// <summary>
    /// 最大效果数量
    /// </summary>
    public const int MAX_EFFECT = 16;
    /// <summary>
    /// sprite像素大小反比，越小图像越大
    /// </summary>
    public const int PIXEL_PERUNIT = 50;

    # endregion

    # region SettingField
    /// <summary>
    /// 文本语言
    /// </summary>
    public static string Language = "zh-cn";
    /// <summary>
    /// 平台
    /// </summary>
    public static string Platform = "windows";
    /// <summary>
    /// 输入方式
    /// </summary>
    public static string Controller = "mouse_keyboard";

    # endregion

    # region HybirdCRT Assembly

    //HybirdCRT热更新程序集
    private static Assembly _hotUpdate = null;
    public static Assembly HotUpdate{
        get{
            if(_hotUpdate == null){
                _hotUpdate = HybirdAssemblyManager.Load("HotUpdate");
            }
            return _hotUpdate;
        } 
    }
    //HybirdCRT热更新实体脚本
    private static Assembly _entityBehaviors = null;
    public static Assembly EntityBehaviors{
        get{
            if(_entityBehaviors == null){
                _entityBehaviors = HybirdAssemblyManager.Load("EntityBehaviors");
            }
            return _entityBehaviors;
        } 
    }
    //HybirdCRT热更新效果脚本
    private static Assembly _effects = null;
    public static Assembly Effects{
        get{
            if(_effects == null){
                _effects = HybirdAssemblyManager.Load("Effects");
            }
            return _effects;
        } 
    }
    
    # endregion

    # region Function

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="sceneName"></param>
    public static void ChangeScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 适配当前文本语言后缀
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string LFormat(string str){
        return str + "_" + Language + ".xml";
    }

    /// <summary>
    /// 读取文本
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ReadText(string path, string name){
        string xmlpath = Path.Combine(AssetPath.assetPath,"Texts", path);
        XDocument doc = XDocument.Load(xmlpath);
        XElement element = doc.Descendants("Text").FirstOrDefault(//筛选name
            e => e.Attribute("name")?.Value == name
        );
        return element?.Value;
    }

    /// <summary>
    /// 读取UI文本，简化写法
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string ReadUIText(string name){
        return ReadText(LFormat("UI"), name).Trim();
    }

    private static INIParser settingFile = null;
    /// <summary>
    /// 读取设置文件,优先读取用户的配置文件，若无则读取默认配置文件，并自动生成用户配置文件
    /// </summary>
    /// <param name="region"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string Setting(string region, string key){
        if(settingFile == null){
            string userpath = Path.Combine(AssetPath.configPath, "Setting.conf");
            string defaultpath = Path.Combine(AssetPath.assetPath, "DefaultSetting.conf");
            if(File.Exists(userpath)){
                settingFile = new INIParser();
                settingFile.Open(userpath);
                Debug.Log("读取用户配置文件:" + userpath);
            }else{
                settingFile = new INIParser();
                settingFile.Open(defaultpath);
                File.Copy(defaultpath, userpath, true); 
            }
        }
        return settingFile.ReadValue(region, key, ""); 
    }

    /// <summary>
    /// 获取配置文件的路径，简化写法，默认从persistentDataPath目录读取
    /// </summary>
    /// <param name="pathname"></param>
    /// <returns></returns>
    public static string GetPath(string pathname, string sourcePath = "ConfigPath"){
        if(sourcePath == "ConfigPath")
            sourcePath = AssetPath.configPath;
        else if(sourcePath == "AssetPath")
            sourcePath = AssetPath.assetPath;
        return string.Format(Setting("Path", pathname), sourcePath);
    }

    /// <summary>
    /// 加载设置
    /// </summary>
    public static void LoadSetting(){
        Language = Setting("MainSetting","language");
        Platform = Setting("MainSetting","platform");
        Controller = Setting("MainSetting","controller");
    }

    # endregion
}
