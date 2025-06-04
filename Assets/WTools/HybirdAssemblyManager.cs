using System.Linq;
using System.Reflection;
using System.IO;
using UnityEngine;

/*
HybirdCLR热更新管理模块
*/
public class HybirdAssemblyManager
{   
    /// <summary>
    /// 加载热更新程序集
    /// </summary>
    /// <returns></returns>
    public static Assembly Load(string assname){
        # if UNITY_EDITOR
        return System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == assname);
        # else
        return Assembly.Load(File.ReadAllBytes(Path.Combine(AssetPath.assetPath, $"{assname}.dll.bytes")));
        # endif
    }
}
