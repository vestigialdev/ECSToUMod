using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod.BuildEngine.ModTools;
using UnityEditor;
using UMod.BuildPipeline;
using UMod.BuildEngine;
using UMod.BuildPipeline.Build;

[UModBuildProcessor(typeof(ECSAssemblyToInclude))]
internal sealed class ECSAssemblyProcessor : BuildEngineProcessor {

    public override void ProcessAsset(BuildContext context, BuildPipelineAsset asset) {
        Debug.Log($"Found ECS assembly to include: {asset.Name} at {asset.RelativePath}");

        var map = AssetDatabase.LoadAssetAtPath<ECSAssemblyToInclude>(asset.RelativePath);
        //Debug.Log($"Result: {map.ECSAssemblyDefinition.text}");

        var name = JsonUtility.FromJson<ASMDEFStub>(map.ECSAssemblyDefinition.text);

        var parentDir = System.IO.Directory.GetParent(Application.dataPath);
        var assemblyPath = System.IO.Path.Combine(parentDir.ToString(), "Library", "ScriptAssemblies", name.name + ".dll");
        Debug.Log($"Assembly path {assemblyPath}");

        var assemblyBytes = System.IO.File.ReadAllBytes(assemblyPath);

        Debug.Log($"Assembly bytes length: {assemblyBytes.Length}");


        var registerResult = context.BuildAssemblies.RegisterAssemblyForBuild(assemblyBytes, false);

        Debug.Log($"Result of RegisterAssemblyForBuild: {registerResult}");

    }
}