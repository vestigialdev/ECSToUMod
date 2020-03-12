using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Startup : MonoBehaviour {

    [MenuItem("Modding/Do setup2")]
    static void Copy2() {
        Debug.Log($"ECSToUMod setting up");

        var one = "Packages/com.vestigial.ecstoumod/ECSToUMod~";
        var absolute = Path.GetFullPath(one);

        Debug.Log($"Absolute: { absolute}");

        try {
            FileUtil.CopyFileOrDirectory(absolute, "Assets/ECSToUMod");
            AssetDatabase.Refresh();
        } catch(System.Exception e) {
            Debug.LogError($"Problem copying: {e.Message}");
            throw;
        }
    }
}
