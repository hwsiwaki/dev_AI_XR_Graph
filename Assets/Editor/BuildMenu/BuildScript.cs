using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    public static void BuildProject(string fileName)
    {
        // 現在の日付と時刻を取得し、フォーマットする
        string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmm");

        // ビルドの出力パス
        string buildPath = $"Builds/{fileName}/OpenXR_MRTK3_{fileName}_{currentDateTime}.apk";

        // 使用するシーンの配列
        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)  // 有効になっているシーンのみ
            .Select(scene => scene.path)
            .ToArray();

        // ビルドターゲット
        BuildTarget buildTarget = BuildTarget.Android;

        // ビルドオプション
        BuildOptions options = BuildOptions.None;

        // BuildPlayerOptionsの設定
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes              = scenes,
            locationPathName    = buildPath,
            target              = buildTarget,
            options             = options
        };

        // ビルドを実行
        BuildPipeline.BuildPlayer(buildPlayerOptions);

        // ビルド完了メッセージ
        Debug.Log($"Finished building project: {buildPath}");
    }
}
