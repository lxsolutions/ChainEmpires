


using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateStoreAssets : EditorWindow
{
    [MenuItem("Tools/Generate Store Assets")]
    public static void ShowWindow()
    {
        GetWindow<GenerateStoreAssets>("Generate Store Assets");
    }

    private void OnGUI()
    {
        GUILayout.Label("Store Asset Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Generate Placeholder Icons"))
        {
            GeneratePlaceholderIcons();
        }

        if (GUILayout.Button("Generate Screenshots"))
        {
            GenerateScreenshots();
        }

        if (GUILayout.Button("Generate All Store Assets"))
        {
            GenerateAllStoreAssets();
        }
    }

    private void GeneratePlaceholderIcons()
    {
        string storeDir = "Assets/Store";
        if (!Directory.Exists(storeDir))
        {
            Directory.CreateDirectory(storeDir);
        }

        // Generate 512x512 icon
        string iconPath = Path.Combine(storeDir, "icon_512x512.png");
        CreatePlaceholderIcon(iconPath, 512, 512, "CE");

        // Generate additional sizes if needed
        CreatePlaceholderIcon(Path.Combine(storeDir, "icon_180x180.png"), 180, 180, "CE");
        CreatePlaceholderIcon(Path.Combine(storeDir, "icon_72x72.png"), 72, 72, "CE");

        AssetDatabase.Refresh();
        Debug.Log("Generated placeholder store icons");
    }

    private void GenerateScreenshots()
    {
        string screenshotDir = "Assets/Store/Screenshots";
        if (!Directory.Exists(screenshotDir))
        {
            Directory.CreateDirectory(screenshotDir);
        }

        // Create placeholder screenshots
        for (int i = 1; i <= 5; i++)
        {
            string screenshotPath = Path.Combine(screenshotDir, $"screenshot_{i}.png");
            CreatePlaceholderScreenshot(screenshotPath, 1920, 1080, $"Scene {i}");
        }

        AssetDatabase.Refresh();
        Debug.Log("Generated placeholder screenshots");
    }

    private void GenerateAllStoreAssets()
    {
        GeneratePlaceholderIcons();
        GenerateScreenshots();

        // Create store description file
        string descriptionPath = "Assets/Store/store_description.md";
        File.WriteAllText(descriptionPath, GetStoreDescription());
        AssetDatabase.Refresh();

        Debug.Log("Generated all store assets");
    }

    private void CreatePlaceholderIcon(string path, int width, int height, string text)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        // Fill with black background
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black;
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Draw white text in center
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = Mathf.Min(width, height) / 3;

        Texture2D textTexture = new Texture2D(width, height);
        Color[] textPixels = new Color[width * height];

        for (int i = 0; i < textPixels.Length; i++)
        {
            textPixels[i] = Color.clear;
        }

        textTexture.SetPixels(textPixels);
        textTexture.Apply();

        // Draw the text
        Texture2D temp = RenderTextToTexture(text, style, new Rect(0, 0, width, height));
        Graphics.CopyTexture(temp, textTexture);

        byte[] pngData = textTexture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
        }

        UnityEngine.Object.DestroyImmediate(texture);
        UnityEngine.Object.DestroyImmediate(textTexture);
        UnityEngine.Object.DestroyImmediate(temp);
    }

    private void CreatePlaceholderScreenshot(string path, int width, int height, string sceneName)
    {
        Texture2D texture = new Texture2D(width, height);

        // Create a gradient background
        Color[] pixels = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            float t = y / (float)height;
            Color color = Color.Lerp(Color.blue, Color.green, t);
            for (int x = 0; x < width; x++)
            {
                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // Draw scene name
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = height / 10;

        Texture2D textTexture = RenderTextToTexture(sceneName, style, new Rect(0, 0, width, height));

        // Combine background and text
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color bgColor = pixels[y * width + x];
                Color txtColor = textTexture.GetPixel(x, y);
                if (txtColor.a > 0)
                {
                    pixels[y * width + x] = Color.Lerp(bgColor, txtColor, 0.5f);
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
        }

        UnityEngine.Object.DestroyImmediate(texture);
        UnityEngine.Object.DestroyImmediate(textTexture);
    }

    private Texture2D RenderTextToTexture(string text, GUIStyle style, Rect rect)
    {
        Texture2D texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[(int)(rect.width * rect.height)];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        RenderTexture renderTex = RenderTexture.GetTemporary((int)rect.width, (int)rect.height, 24);
        renderTex.Create();
        Graphics.Blit(texture, renderTex);

        RenderTexture.active = renderTex;
        Texture2D result = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

        style.normal.textColor = Color.white;

        int oldAlignment = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;

        GUI.Label(rect, text, style);
        result.ReadPixels(new Rect(0, 0, rect.width, rect.height), 0, 0);

        GUI.skin.label.alignment = oldAlignment;

        RenderTexture.ReleaseTemporary(renderTex);
        return result;
    }

    private string GetStoreDescription()
    {
        return @"
# Chain Empires Store Description

## Game Overview
Chain Empires is a revolutionary 3rd-person mobile strategy game that combines elements from Age of Empires, Civilization, Diablo, and Starcraft with modern P2E economics. Players build their empires through resource management, unit training, base upgrades, and strategic combat.

## Key Features
- **Turn-based PvP/PvE Combat**: Strategic battles against AI and other players
- **Base Building & Upgrades**: Customize your empire with unique structures and defenses
- **Resource Management**: Collect and allocate resources for growth and expansion
- **Adaptive AI Waves**: Intelligent enemy behavior with learning and diplomacy systems
- **Narrative Echo Realms**: Immersive story-driven gameplay with branching paths
- **AR Buffs & NFT Cosmetics**: Augmented reality enhancements and customizable items
- **Solana Web3 Integration**: True ownership of in-game assets via blockchain technology

## Gameplay Screenshots
![Screenshot 1](Screenshots/screenshot_1.png)
![Screenshot 2](Screenshots/screenshot_2.png)
![Screenshot 3](Screenshots/screenshot_3.png)

## System Requirements
- **Android**: Version 8.0 or higher, 4GB RAM recommended
- **iOS**: iOS 13 or higher, iPhone 7 or newer recommended

## Contact Information
For support and inquiries: contact@chainempires.com
";
    }
}

