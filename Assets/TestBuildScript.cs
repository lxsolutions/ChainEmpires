





using UnityEngine;
using UnityEditor;

namespace ChainEmpires.Build
{
    public class TestBuildScript : MonoBehaviour
    {
        [MenuItem("Test/Run Build Script")]
        public static void RunBuildTest()
        {
            Debug.Log("Testing build script functionality...");

            // Test Android APK build
            Debug.Log("Building Android APK...");
            BuildScript.BuildAlphaAPK();

            // Test iOS IPA build
            Debug.Log("Building iOS IPA...");
            BuildScript.BuildAlphaIPA();
        }
    }
}





