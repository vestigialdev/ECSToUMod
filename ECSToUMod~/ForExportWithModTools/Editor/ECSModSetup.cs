using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;
using UnityEditor;
using UMod.BuildEngine;
using UMod.ModTools.Export;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[InitializeOnLoad]
public class ECSModSetup {
    static ECSModSetup() {
        UMod.BuildEngine.ModCreate.OnModCreated += CreateECSFolderForNewMod;
    }

    static public void CreateECSFolderForNewMod(ModCreateArgs args) {

        //Create ECS folder
        var parentPath = System.IO.Directory.GetParent(args.relativePath);
        var projectDir = System.IO.Directory.GetParent(Application.dataPath).ToString();

        //Create Assembly Definition file for ECS content
        var ecsPathGUID = AssetDatabase.CreateFolder(parentPath.ToString(), args.modName + "_ECS");
        var ecsPath = AssetDatabase.GUIDToAssetPath(ecsPathGUID);
        //Debug.Log($"ECSPath is {ecsPath}");

        var dirName = args.modName + "_ECS";
        var filename = dirName + ".asmdef";

        var ecsAsmdefSourcePath = Application.dataPath + "/Mod Tools/Editor/Resources/ECSAssembly.asmdef.txt";
        var ecsAsmdefDestPath = Application.dataPath + "/" + dirName + "/" + filename;

        //Debug.Log($"Source path {ecsAsmdefSourcePath}");
        //Debug.Log($"Output path {ecsAsmdefDestPath}");

        var ecsASMDEF = System.IO.File.ReadAllText(ecsAsmdefSourcePath);
        var newASMDEF = ecsASMDEF.Replace("$ECSAssembly$", dirName);

        //Write modified ASMDEF to mod ecs folder
        System.IO.File.WriteAllText(ecsAsmdefDestPath, newASMDEF);


        //Make a stub .cs file in the mod directory to ensure it gets packed as "has scripts"

        //Make a custom ScriptableObject asset that the Processor will pick up and ensure the ECS assembly is included


        AssetDatabase.Refresh();
    }
}