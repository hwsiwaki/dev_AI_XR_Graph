using UnityEditor;
using UnityEngine;

public class AndroidAPILevelScript
{
    public static void ChangeAndroidSdkVersion(AndroidSdkVersions minSdkVersion, AndroidSdkVersions tergetSdkVersion)
    {
        PlayerSettings.Android.minSdkVersion = minSdkVersion;
        PlayerSettings.Android.targetSdkVersion = tergetSdkVersion;

        // 変更をコンソールに出力
        Debug.Log($"Android API Levels updated: Min = {minSdkVersion}, Target = {tergetSdkVersion}");
    }
}
