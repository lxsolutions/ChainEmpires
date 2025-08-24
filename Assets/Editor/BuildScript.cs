


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BuildScript
{
    static readonly string[] SCENES = {
        "Assets/Scenes/AlphaScene.unity", // Main game scene
        "Assets/Scenes/MainMenu.unity"     // Menu scene
    };

    [MenuItem("Build/Build Alpha APK")]
    public static void BuildAlphaAPK()
    {
        BuildPlayer(BuildTarget.Android, "ChainEmpires_Alpha.apk");
    }

    [MenuItem("Build/Build Alpha AAB")]
    public static void BuildAlphaAAB()
    {
        BuildPlayer(BuildTarget.Android, "ChainEmpires_Alpha.aab", true);
    }

    [MenuItem("Build/Build Alpha iPA")]
    public static void BuildAlphaIPA()
    {
        BuildPlayer(BuildTarget.iOS, "ChainEmpires_Alpha.ipa");
    }

    private static void BuildPlayer(BuildTarget target, string filename, bool isAndroidBundle = false)
    {
        string outputPath = $"Builds/{target}/{filename}";

        EditorUserBuildSettings.SwitchActiveBuildTarget(target);

        BuildOptions options = BuildOptions.None;
        if (isAndroidBundle)
        {
            options |= BuildOptions.AcceptExternalModificationsToPlayer;
        }

        GenericBuild(options, outputPath);
    }

    private static void GenericBuild(BuildOptions buildOptions, string outputPath, BuildTarget target)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(target);

        BuildReport report = BuildPipeline.BuildPlayer(SCENES, outputPath, target, buildOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + outputPath);
        }
        else
        {
            Debug.LogError("Build failed");
            foreach (var step in report.steps)
            {
                Debug.LogError($"Step: {step.name}, Result: {step.result}");
            }
        }
    }
}


