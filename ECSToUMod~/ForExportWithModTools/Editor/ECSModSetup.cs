using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;
using UnityEditor;
using UMod.BuildEngine;
using UMod.ModTools.Export;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class ASMDEFStub {
    public string name;
}

[InitializeOnLoad]
static class ECSModSetup {
    static ECSModSetup() {
        ModCreate.OnModCreated += CreateECSFolderForNewMod;
    }

    static ModCreateArgs Args;
    static string ModName => Args.modName;
    static string ECSName => $"{ModName}_ECS";
    static string AsmDefFilename => $"{ECSName}.asmdef";
    static string DummyCSFilename => "Dummy.cs";

    static string ModDirectoryRelativePath => Args.relativePath;
    static string AsmDefFileRelativePath => $"{ECSDirectoryRelativePath}/{AsmDefFilename}";
    static string AsmDefFileAbsolutePath => System.IO.Path.Combine(ProjectDirAbsolutePath, ECSDirectoryRelativePath, AsmDefFilename);
    static string AssemblyRefRelativePath => $"{ModDirectoryRelativePath}/{ECSName}Ref.asset";

    static string ProjectDirAbsolutePath => System.IO.Directory.GetParent(Application.dataPath).ToString();
    static string ECSDirectoryRelativePath => System.IO.Path.Combine($"{ModParentDirectoryRelativePath}", $"{ECSName}");
    static string ModParentDirectoryRelativePath => System.IO.Directory.GetParent(Args.relativePath).ToString();

    static void CreateECSFolderForNewMod(ModCreateArgs args) {
        Args = args;

        //Only create ECS functionality when user consents
        if(!EditorUtility.DisplayDialog("ECS to UMod", $"Set up the mod '{ModName}' for ECS?", "Enable ECS", "Decline")) {
            return;
        }


        //Create Assembly Definition file for ECS folder
        var asmdef = new ASMDEFStub() {
            name = $"{ModName}_ECS",
        };

        //Convert ASMDEF stub to JSON text representation
        var asmdefText = JsonUtility.ToJson(asmdef, true);

        Debug.Log($"Writing asmdef to {AsmDefFileRelativePath}");

        //Create directory for ECS code
        AssetDatabase.CreateFolder(ModParentDirectoryRelativePath, ECSName);

        //Write modified ASMDEF to mod ecs folder
        System.IO.File.WriteAllText(AsmDefFileAbsolutePath, asmdefText);

        //Make a dummy .cs file in the mod directory to ensure it gets packed as "has scripts"
        System.IO.File.WriteAllText(System.IO.Path.Combine(ProjectDirAbsolutePath, ModDirectoryRelativePath, DummyCSFilename), "");


        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //Make a custom ScriptableObject asset that the Processor will pick up and ensure the ECS assembly is included
        var includeRef = ECSAssemblyToInclude.CreateInstance<ECSAssemblyToInclude>();
        includeRef.ECSAssemblyDefinition = AssetDatabase.LoadAssetAtPath<TextAsset>(AsmDefFileRelativePath);

        AssetDatabase.CreateAsset(includeRef, Args.relativePath + "/ECSAssembnlyRef.asset");
        AssetDatabase.SaveAssets();
    }
}