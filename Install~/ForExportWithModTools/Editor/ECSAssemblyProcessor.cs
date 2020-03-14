using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod.BuildEngine.ModTools;
using UnityEditor;
using UMod.BuildPipeline;
using UMod.BuildEngine;
using UMod.BuildPipeline.Build;

[UModBuildProcessor(typeof(ECSAssemblyToInclude))]
public class ECSAssemblyProcessor : BuildEngineProcessor {

    public override void ProcessAsset(BuildContext context, BuildPipelineAsset asset) {
        ECSToModGeneral.Print($"Found ECS assembly to include: {asset.Name} at {asset.RelativePath}");

        var map = AssetDatabase.LoadAssetAtPath<ECSAssemblyToInclude>(asset.RelativePath);
        var name = JsonUtility.FromJson<ASMDEFStub>(map.ECSAssemblyDefinition.text);

        var parentDir = System.IO.Directory.GetParent(Application.dataPath);
        var assemblyPath = System.IO.Path.Combine(parentDir.ToString(), "Library", "ScriptAssemblies", name.name + ".dll");
        ECSToModGeneral.Print($"Assembly path {assemblyPath}");

        var assemblyBytes = System.IO.File.ReadAllBytes(assemblyPath);

        ECSToModGeneral.Print($"Assembly bytes length: {assemblyBytes.Length}");


        var registerResult = context.BuildAssemblies.RegisterAssemblyForBuild(assemblyBytes, false);

        ECSToModGeneral.Print($"Result of RegisterAssemblyForBuild: {registerResult}");

    }


}
