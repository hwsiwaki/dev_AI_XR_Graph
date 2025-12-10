using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

public class OpenXRScript
{
    public static void SetOpenXR(List<Type> featureTypes)
    {
        SetXRPluginProviders();
        DeleteAllFeatures();

        foreach (var featureType in featureTypes)
        {
            EnableFeature(featureType);
        }
    }

    private static void SetXRPluginProviders()
    {
        var currentGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        XRGeneralSettings currentSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(currentGroup);
        XRManagerSettings managerSettings = currentSettings.AssignedSettings;
        if (managerSettings != null)
        {
            var loader = managerSettings.activeLoaders.FirstOrDefault(loader => loader is OpenXRLoader);
            if (loader == null)
            {
                loader = ScriptableObject.CreateInstance<OpenXRLoader>();
                managerSettings.TryAddLoader(loader);
            }
        }
        EditorUtility.SetDirty(currentSettings);
        AssetDatabase.SaveAssets();
    }

    protected static void EnableFeature(Type featureType)
    {
        var openXRSettings = OpenXRSettings.ActiveBuildTargetInstance;
        var feature = openXRSettings.GetFeature(featureType);
        if (feature != null && !feature.enabled)
        {
            feature.enabled = true;
            EditorUtility.SetDirty(feature);
            AssetDatabase.SaveAssets();
            Debug.Log($"Enable Feature: {feature.name}");
        }
    }

    private static void DeleteAllFeatures()
    {
        var openXRSettings = OpenXRSettings.ActiveBuildTargetInstance;
        var features = openXRSettings.GetFeatures();
        foreach (var feature in features)
        {
            if (feature.enabled)
            {
                feature.enabled = false;
                EditorUtility.SetDirty(feature);
                AssetDatabase.SaveAssets();
                Debug.Log($"Delete Feature: {feature.name}");
            }
        }
    }
}
