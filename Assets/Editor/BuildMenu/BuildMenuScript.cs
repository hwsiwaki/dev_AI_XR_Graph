using Meta.XR;
using Qualcomm.Snapdragon.Spaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.XR.Hands.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.OpenXR.Features.MetaQuestSupport;

public class BuildMenuScript
{
    [MenuItem("Builds/Build for MetaQuest")]
    private static void UseCustomSymbol1()
    {
        SetBuild(BuildSettings.GetBuildInfo(BuildType.MetaQeust));
    }

    [MenuItem("Builds/Build for Digilens")]
    private static void UseCustomSymbol2()
    {
        SetBuild(BuildSettings.GetBuildInfo(BuildType.Digilens));
    }

    private static void SetBuild(BuildInfo buildInfo)
    {
        AndroidManifestScript.ReplaceManifest(buildInfo.CustomManifest);

        SymbolScript.ChangeSymbols(buildInfo.Symbol);

        OpenXRScript.SetOpenXR(buildInfo.featureTypes);

        AndroidAPILevelScript.ChangeAndroidSdkVersion
        (
            buildInfo.minSdkVersion,
            buildInfo.targetSdkVersion
        );

        BuildScript.BuildProject(buildInfo.BuildName.ToString());
    }

    [Serializable]
    public class BuildSettings
    {
        public static List<BuildInfo> BuildInfos = new List<BuildInfo>
        {
            new BuildInfo
            (
                BuildType.MetaQeust,
                "Assets/Plugins/Android/AndroidManifest/AndroidManifest_MetaQuest.xml",
                "BUILD_FOR_METAQUEST",
                AndroidSdkVersions.AndroidApiLevel32,
                AndroidSdkVersions.AndroidApiLevel32,
                new List<Type>
                {
                    typeof(HandInteractionProfile),
                    typeof(MicrosoftHandInteraction),
                    typeof(OculusTouchControllerProfile),
                    typeof(SpacesMicrosoftMixedRealityMotionControllerProfile),

                    typeof(HandTracking),

                    typeof(MetaQuestFeature),
                    typeof(MetaXRFeature),
                    typeof(ARSessionFeature),
                    typeof(ARCameraFeature)
                }
            ),
            new BuildInfo
            (
                BuildType.Digilens,
                "Assets/Plugins/Android/AndroidManifest/AndroidManifest_Digilens.xml",
                "BUILD_FOR_DIGILENS",
                AndroidSdkVersions.AndroidApiLevel29,
                AndroidSdkVersions.AndroidApiLevel31,
                new List<Type>
                {
                    typeof(HandInteractionProfile),
                    typeof(MicrosoftHandInteraction),
                    typeof(OculusTouchControllerProfile),
                    typeof(SpacesMicrosoftMixedRealityMotionControllerProfile),

                    typeof(HandTracking),

                    typeof(BaseRuntimeFeature)
                }
            )
        };

        public static BuildInfo GetBuildInfo(BuildType buildType)
        {
            return BuildInfos.FirstOrDefault(item => item.BuildName == buildType);
        }

        public static List<string> GetAllSymbol()
        {
            List<string> results = new();

            foreach(var buildInfo in BuildInfos)
            {
                if (!results.Contains(buildInfo.Symbol))
                {
                    results.Add(buildInfo.Symbol);
                }
            }

            return results;
        }
    }

    [Serializable]
    public class BuildInfo
    {
        public BuildType BuildName;
        public string CustomManifest;
        public string Symbol;
        public AndroidSdkVersions minSdkVersion;
        public AndroidSdkVersions targetSdkVersion;
        public List<Type> featureTypes;

        public BuildInfo
        (
            BuildType BuildName,
            string CustomManifest,
            string Symbol,
            AndroidSdkVersions minSdkVersion,
            AndroidSdkVersions targetSdkVersion,
            List<Type> featureTypes
        )
        {
            this.BuildName = BuildName;
            this.CustomManifest = CustomManifest;
            this.Symbol = Symbol;
            this.minSdkVersion = minSdkVersion;
            this.targetSdkVersion = targetSdkVersion;
            this.featureTypes = featureTypes;
        }
    }

    public enum BuildType
    {
        MetaQeust,
        Digilens,
    }
}