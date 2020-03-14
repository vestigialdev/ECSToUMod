using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Setup {

    static string SetupCompleteKey = "ECSToUModEditor_SetupComplete";

    [MenuItem("Tools/ECSToUMod/Reinstall")]
    static void Reinstall() {
        EditorPrefs.SetBool(SetupCompleteKey, false);
        Install();
    }

    [InitializeOnLoadMethod]
    static void Install() {

        //Don't automatically install if setup has already been completed
        if(EditorPrefs.GetBool(SetupCompleteKey, false)) {
            return;
        }

        Debug.Log($"ECSToUMod Install()");

        var setupPathRelative = "Packages/com.vestigial.ecstoumod/Install~";
        var installPathAbsolute = Path.GetFullPath(setupPathRelative);

        try {
            var destinationPath = "Assets/ECSToUmodFiles";
            Debug.Log($"Copying files from {installPathAbsolute} to {destinationPath}");

            //Exists?
            var existingFolder = AssetDatabase.LoadMainAssetAtPath(destinationPath);
            if(null != existingFolder) {
                throw new System.Exception($"Can't reinstall if the folder already exists, try deleting {destinationPath} first");
            }

            //Actually copy the folder
            FileUtil.CopyFileOrDirectory(installPathAbsolute, destinationPath);

            //ping the new folder
            var newlyInstalledFolder = AssetDatabase.LoadMainAssetAtPath(destinationPath);
            EditorGUIUtility.PingObject(newlyInstalledFolder);

            AssetDatabase.Refresh();
            Debug.Log($"ECSToUMod install complete");
            EditorPrefs.SetBool(SetupCompleteKey, true);

        } catch(System.Exception e) {
            Debug.LogError($"Problem installing: {e.Message}");
            return;
        }

    }
}
