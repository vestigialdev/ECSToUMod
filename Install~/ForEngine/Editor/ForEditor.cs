using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using System;
using UnityEditor;
using System.Reflection;
using Unity.Scenes.Editor;

public partial class ECSToUModEditor {

    //See if the TypeManager has been initialized yet 
    static bool TypeManagerIsInitialized => (bool)typeof(TypeManager).GetField("s_Initialized", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

    static string IsWriteGroupEnabledKey = "IsWriteGroupEnabled";
    static bool IsWriteGroupEnabled => PlayerPrefs.GetInt(IsWriteGroupEnabledKey, 0) != 0;
    
    [MenuItem("Tools/ECSToUMod/Open problematic Unity files for writing")]
    static void OpenFilesForChange() {

        if(!IsWriteGroupEnabled) {
            Debug.Log($"Changing the Unity ECS files is for WriteGroup functionality, which is currently {IsWriteGroupEnabled}");
            return;
        }

        string path1 = "TypeDependencyCache.cs";
        string path2 = "CompanionGameObject.cs";
        var assetPaths = new List<string>();

        foreach(var assetPath in AssetDatabase.GetAllAssetPaths()) {
            if(assetPath.EndsWith(path1)) {
                var script = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
                if(script != null) {
                    AssetDatabase.OpenAsset(script, 12, 4);
                    //break;
                }
            }
            if(assetPath.EndsWith(path2)) {
                var script = (MonoScript)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript));
                if(script != null) {
                    AssetDatabase.OpenAsset(script, 13, 4);
                    //break;
                }
            }
        }
    }

    [MenuItem("Tools/ECSToUMod/Toggle WriteGroup functionality")]
    static void ToggleWriteGroupFunctionality() {
        var newValue = !IsWriteGroupEnabled;
        PlayerPrefs.SetInt(IsWriteGroupEnabledKey, newValue ? 1 : 0);
        Debug.LogWarning($"ECSToUMod: WriteGroup functionality is now {newValue}");
    }

    static void PrintWriteGroupInstructions() {
        Debug.LogWarning($"ECSToUMod: Comment out [InitializeOnLoad] on line 12 of TypeDependencyCache.cs");
        Debug.LogWarning($"ECSToUMod: Comment out [InitializeOnLoad] on line 13 of AttachToEntityClonerInjection.cs");
        if(!EditorUtility.DisplayDialog("ECS to UMod", $"Open files to edit in VS?", "Open in text editor", "Decline")) {
            EditorUtility.DisplayDialog("ECS to UMod", $"At any time, you can use Tools/ECSToUMod/Open files for writing", "Okay");
            return;
        }

        OpenFilesForChange();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void AddECSTypesInEditor() {

        ECSToModGeneral.Print("ECSToUMod: ECSToUModEditor.AddECSTypesInEditor() called automatically by RuntimeInitializeLoadType.AfterAssembliesLoaded");

        if(null == ECSToUMod.ModHosts) {
            throw new Exception($"ECSToUMod: ECSToUMod.ModHosts[] needs to be populated before ECSToUmod.AddECSTypesInEditor() is called automatically via the [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)] attribute");
        }

        if(IsWriteGroupEnabled) {

            //Ensure the TypeManager has not been initialized yet
            if(TypeManagerIsInitialized) {
                Debug.LogError($"ECSToUMod: WriteGroup functionality is {IsWriteGroupEnabled} but TypeManager has already been initialized. WriteGroups will be ignored!");
                PrintWriteGroupInstructions();
            }

            //TypeManager.Initialize() was interfered with, so we may need to call it manually here
            //Or maybe not!
            //TypeManager.Initialize();

        } else {

            //TypeDependencyCache and AttachToEntityClonerInjection both call TypeManager.Initialize() very early, in the Editor
            //However, using  TypeManager.AddNewComponentTypes(), we can still include loaded mod code 
            //Note- this way doesn't respect WriteGroup attributes! 

            //Create a list of all interfaces that the TypeManager needs to know about
            List<System.Type> DOTSInterfaces = new List<System.Type> {
                typeof(IBufferElementData),
                typeof(IComponentData),
                typeof(ISharedComponentData),
                typeof(IConvertGameObjectToEntity),
            };

            //Iterate mod hosts looking for types to add
            foreach(var host in ECSToUMod.ModHosts) {

                if(!host.IsModLoaded) {
                    continue;
                }

                //Find all mod assemblies that reference the Entities namespace
                foreach(var modAssembly in host.ScriptDomain.Assemblies.Where(assembly => TypeManager.IsAssemblyReferencingEntities(assembly.RawAssembly))) {

                    foreach(var DOTSInterface in DOTSInterfaces) {
                        //Find all type definitions in the loaded mod code that implement this interface (for example MyComponent: IComponentData) and grab the actual underlying System.Type via .RawType
                        var subTypes = modAssembly.FindAllSubTypesOf(DOTSInterface).Select(subtype => subtype.RawType);

                        //Add it to the TypeManager using this special editor-only function
                        TypeManager.AddNewComponentTypes(subTypes.ToArray());

                        if(TypeManagerIsInitialized) {
                            bool displayInstructions = false;
                            foreach(var type in subTypes) {
                                var attribute = type.GetCustomAttribute<WriteGroupAttribute>();
                                if(null == attribute) {
                                    continue;
                                }

                                Debug.LogWarning($"ECSToUMod: Assembly {modAssembly.Name} has a type {type.Name} with the WriteGroup attribute");
                                displayInstructions = true;
                            }

                            if(displayInstructions) {
                                Debug.LogError($"To enable WriteGroup functionality in the editor with UMod, toggle Tools/ECSToUMod/Toggle writegroup functionality");
                            }
                        }
                    }
                }
            }
        }
    }
}