using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using System;
using UnityEditor;

public partial class ECSToUModEditor {

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void AddECSTypesInEditor() {

        ECSToModGeneral.Print("RuntimeInitializeLoadType.AfterAssembliesLoaded: automatically calling ECSToUModEditor.AddECSTypesInEditor()");

        if(null == ECSToUMod.ModHosts) {
            throw new Exception($"ECSToMod.ModHosts[] needs to be populated before ECSToUmod.AddECSTypesInEditor() is called automatically via the [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)] attribute");
        }

        //In editor, TypeManager.Initialize() gets called earlier than usual, by TypeDependencyCache as well as AttachToEntityClonerInjection, before we have any hope of loading mod code
        //We need to explicitly inject types into the TypeManager using some editor-specific code, so this must be wrapped in the #if UNITY_EDITOR ... #endif processor (or do it in an Editor script)

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
                }
            }
        }
    }
}
