
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Diagnostics;

public class PerformanceTestScript : EditorWindow
{
    private string testScenePath = "Assets/Scenes/AlphaScene.unity";
    private int enemyCount = 300;
    private int waveCount = 10;
    private int playerCount = 5;
    private float testDurationMinutes = 20f;

    [MenuItem("Tools/Performance Test")]
    public static void ShowWindow()
    {
        GetWindow<PerformanceTestScript>("Performance Test");
    }

    private void OnGUI()
    {
        GUILayout.Label("Performance Test Configuration", EditorStyles.boldLabel);
        GUILayout.Space(10);

        testScenePath = EditorGUILayout.TextField("Test Scene Path", testScenePath);
        enemyCount = EditorGUILayout.IntField("Enemy Count", enemyCount);
        waveCount = EditorGUILayout.IntField("Wave Count", waveCount);
        playerCount = EditorGUILayout.IntField("Player Count", playerCount);
        testDurationMinutes = EditorGUILayout.FloatField("Test Duration (minutes)", testDurationMinutes);

        if (GUILayout.Button("Run Performance Test"))
        {
            RunPerformanceTest();
        }
    }

    private void RunPerformanceTest()
    {
        // Save current scene
        string currentScenePath = Application.dataPath + "/CurrentScene.unity";
        EditorApplication.SaveCurrentSceneAs(currentScenePath);

        // Open test scene
        EditorApplication.OpenScene(testScenePath);
        Debug.Log("Opened test scene: " + testScenePath);

        // Wait for scene to load
        EditorApplication.update += CheckSceneLoaded;
    }

    private bool sceneLoaded = false;

    private void CheckSceneLoaded()
    {
        if (EditorApplication.timeSinceStartup > 5f)
        {
            sceneLoaded = true;
            EditorApplication.update -= CheckSceneLoaded;

            // Start performance test coroutine
            EditorApplication.update += RunTestCoroutine;
        }
    }

    private float startTime;
    private Stopwatch stopwatch;
    private StreamWriter logFile;

    private void RunTestCoroutine()
    {
        if (!sceneLoaded) return;

        startTime = Time.realtimeSinceStartup;
        stopwatch = new Stopwatch();
        stopwatch.Start();

        // Create log file
        string logPath = "PerformanceTest_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        logFile = new StreamWriter(logPath);
        logFile.WriteLine("Timestamp,FrameRate,FPS,GCTime,MemoryUsage,BatteryLevel");

        // Start test
        EditorApplication.update += PerformanceUpdate;

        Debug.Log("Performance test started. Duration: " + testDurationMinutes + " minutes");
    }

    [MenuItem("Tools/Run Performance Test with High Load")]
    public static void RunPerformanceTestWithHighLoad()
    {
        PerformanceTestScript window = GetWindow<PerformanceTestScript>("Performance Test");

        // Set high load parameters
        window.enemyCount = 500;
        window.waveCount = 15;
        window.playerCount = 10;
        window.testDurationMinutes = 30f;

        window.RunPerformanceTest();
    }

    private void PerformanceUpdate()
    {
        float elapsedTime = Time.realtimeSinceStartup - startTime;

        if (elapsedTime >= testDurationMinutes * 60f)
        {
            EndTest();
            return;
        }

        // Log performance metrics
        float frameRate = 1f / Time.deltaTime;
        float fps = Mathf.Round(frameRate);
        float gcTime = GC.GetTotalMemory(false) / (1024f * 1024f); // MB

        string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
        logFile.WriteLine($"{timestamp},{frameRate},{fps},{gcTime},N/A,N/A");

        // Log to console
        Debug.Log($"Performance: {timestamp} | FPS: {fps} | GC Time: {gcTime}MB");

        // Check for spikes > 100ms
        if (Time.deltaTime > 0.1f)
        {
            Debug.LogWarning("Frame time spike detected: " + Time.deltaTime + " seconds");
        }
    }

    private void EndTest()
    {
        stopwatch.Stop();
        EditorApplication.update -= PerformanceUpdate;

        // Close log file
        if (logFile != null)
        {
            logFile.Close();
            Debug.Log("Performance test completed. Log saved to: " + logFile.BaseStream.Name);
        }

        // Restore original scene
        EditorApplication.OpenScene(Application.dataPath + "/CurrentScene.unity");
        Debug.Log("Restored original scene");

        // Show summary
        float duration = stopwatch.Elapsed.TotalSeconds;
        Debug.Log($"Performance test completed in {duration} seconds ({testDurationMinutes} minutes requested)");
        Debug.Log($"Average FPS: {1f / (duration / 60f)}");
    }
}
