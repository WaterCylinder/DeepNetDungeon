using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetPath : MonoBehaviour
{
    public static string configPath{
        get{
            //PC平台
            if(Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.LinuxPlayer
            || Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.LinuxEditor
            || Application.platform == RuntimePlatform.OSXEditor){
                return Application.persistentDataPath;
            }
            //移动平台
            if(Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer){
                return Application.persistentDataPath;
            }
            return Application.persistentDataPath;
        }
    }

    public static string assetPath{
        get{
            //PC平台
            if(Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.LinuxPlayer
            || Application.platform == RuntimePlatform.OSXPlayer
            || Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.LinuxEditor
            || Application.platform == RuntimePlatform.OSXEditor){
                return Application.streamingAssetsPath;
            }
            if(Application.platform == RuntimePlatform.Android
            || Application.platform == RuntimePlatform.IPhonePlayer){
                return Application.persistentDataPath;
            }
            return Application.streamingAssetsPath;
        }
    }

    public static string resourcesPath{
        get{
            return Path.Combine(Application.dataPath, "Resources");
        }
    }
}
