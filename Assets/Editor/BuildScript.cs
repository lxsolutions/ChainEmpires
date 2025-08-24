


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

        // Add compression and mock signing for alpha builds
        PlayerSettings.Android.useCustomKeystore = false; // Mock signing for alpha
        PlayerSettings.Android.keystoreName = "mock_keystore";
        PlayerSettings.Android.keystorePass = "mock_pass";
        PlayerSettings.Android.keyaliasName = "mock_keyalias";
        PlayerSettings.Android.keyaliasPass = "mock_pass";

        // Enable compression for all platforms
        if (target == BuildTarget.Android)
        {
            PlayerSettings.Android.buildApkByGradle = true;
            PlayerSettings.Android.minifyWithProGuardScript = false; // Disable minification for alpha
            PlayerSettings.Android.deploymentBundle = isAndroidBundle;
        }
        else if (target == BuildTarget.iOS)
        {
            PlayerSettings.iOS.buildNumber = "1";
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.Latest;
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
            LogBuildSize(outputPath);
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

    private static void LogBuildSize(string outputPath)
    {
        if (System.IO.File.Exists(outputPath))
        {
            long size = new System.IO.FileInfo(outputPath).Length;
            string readableSize = BytesToString(size);
            Debug.Log($"Build size: {readableSize} ({size} bytes)");
        }
    }

    private static string BytesToString(long byteCount)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        double number = byteCount;

        while (number >= 1024 && counter < suffixes.Length - 1)
        {
            number /= 1024;
            counter++;
        }

        return $"{number:N2} {suffixes[counter]}";
    }
}


