using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Startup : MonoBehaviour {

    [MenuItem("Tools/ECSToUMod/Do setup")]
    static void Setup() {
        Debug.Log($"ECSToUMod setting up");

        var one = "Packages/com.vestigial.ecstoumod/ECSToUmod~";
        var absolute = Path.GetFullPath(one);

        //Debug.Log($"Absolute: { absolute}");

        try {
            FileUtil.CopyFileOrDirectory(absolute, "Assets/ECSToUmodFiles");
            AssetDatabase.Refresh();
        } catch(System.Exception e) {
            Debug.LogError($"Problem copying: {e.Message}");
            throw;
        }
    }
}
