using System.IO;
using UnityEditor;
using UnityEngine;

public class AndroidManifestScript
{
    private const string MainManifestPath = "Assets/Plugins/Android/AndroidManifest.xml";

    public static void ReplaceManifest(string customManifestPath)
    {
        if (File.Exists(customManifestPath))
        {
            File.Copy(customManifestPath, MainManifestPath, true);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError($"Custom AndroidManifest file does not exist: {customManifestPath}");
        }
    }
}
