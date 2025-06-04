# if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class AssetBundleBuilder : Editor
{   
    //当前构建平台
    public static BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

    [MenuItem("AssetBundle/打包所有资源")]
    public static void BuildAll(){
        string path = EditorUtility.SaveFolderPanel("保存目录", AssetPath.assetPath, "");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        if(!Directory.Exists(path)){
            Directory.CreateDirectory(path);
        }
        BuildPipeline.BuildAssetBundles(
            path, 
            BuildAssetBundleOptions.None, 
            buildTarget
        );
        AssetDatabase.Refresh();
        Debug.Log($"打包成功:on {path}");
    }

    [MenuItem("AssetBundle/独立打包选中资源")]
    public static void BuildSignal(){
        UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        if(assets.Length == 0){
            return;
        }
        string path = EditorUtility.SaveFolderPanel("保存目录", AssetPath.assetPath, "");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        if(!Directory.Exists(path)){
            Directory.CreateDirectory(path);
        }
        //创建打包列表
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        foreach(UnityEngine.Object ass in assets){
            string bundleName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(ass));
            AssetBundleBuild build = new();
            build.assetBundleName = $"{bundleName}_{ass.GetType().Name}";
            build.assetNames = new string[]{AssetDatabase.GetAssetPath(ass)};
            builds.Add(build);
        }
        //按打包列表打包
        BuildPipeline.BuildAssetBundles(
            path, 
            builds.ToArray(),
            BuildAssetBundleOptions.None,
            buildTarget
        );
        Debug.Log($"独立打包选中资源打包成功:on {path}");
    }

    [MenuItem("AssetBundle/打包选中资源")]
    public static void Build(){
        UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        if(assets.Length == 0){
            return;
        }
        string path = EditorUtility.SaveFolderPanel("保存目录", AssetPath.assetPath, "");
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        if(!Directory.Exists(path)){
            Directory.CreateDirectory(path);
        }
        //创建打包列表
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        foreach(Object ass in assets){
            string assPath = AssetDatabase.GetAssetPath(ass);
            AssetImporter importer = AssetImporter.GetAtPath(assPath);
            string bundleName = $"{importer.assetBundleName}{(string.IsNullOrEmpty(importer.assetBundleVariant) ? "" : "_"+importer.assetBundleVariant)}";
            bool isExist = false;
            for(int i = 0; i < builds.Count; i++){
                if(builds[i].assetBundleName == bundleName){
                    AssetBundleBuild b = builds[i];
                    b.assetNames = b.assetNames.Append(assPath).ToArray();
                    builds[i] = b;
                    isExist = true;
                    break;
                }
            }
            if(isExist){
                continue;
            }
            AssetBundleBuild build = new();
            build.assetBundleName = bundleName;
            build.assetNames = new string[]{assPath};
            builds.Add(build);
        }
        //按打包列表打包
        BuildPipeline.BuildAssetBundles(
            path, 
            builds.ToArray(),
            BuildAssetBundleOptions.ForceRebuildAssetBundle,
            buildTarget
        );
        Debug.Log($"选中资源打包成功:on {path}");
    }
}

# endif