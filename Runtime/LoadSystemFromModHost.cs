using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMod;
using System.Linq;
using Unity.Entities;
using Unity.Jobs;
using System;

public class ECSToUMod {
    public static ModHost[] ModHosts;

    public static void LoadSystemsFromModHosts() {

        if(null == ModHosts) {
            Debug.LogError("ECSToUmod.ModHosts needs to be set before calling ECSToUmod.LoadSystemsFromModHosts()");
            return;
        }

        //ECS systems in mod code are not created/updated automatically so we must create them and add them to the update groups ourselves

        //Find the Systems to add 
        foreach(var host in ModHosts) {
            if(!host.IsModLoaded) {
                continue;
            }

            foreach(var assembly in host.ScriptDomain.Assemblies) {

                //Only looking for Systems of type SystemBase, since ComponentSystem/JobComponentSystem look like they'll be deprecated
                foreach(var systemType in assembly.FindAllSubTypesOf<SystemBase>()) {

                    //Find the [UpdateInGroup] attribute, if one exists, for the System
                    //This holds which ComponentSystemGroup the System should be added to (InitializationSystemGroup, SimulationSystemGroup, etc)
                    UpdateInGroupAttribute attribute = (UpdateInGroupAttribute)Attribute.GetCustomAttribute(systemType.RawType, typeof(UpdateInGroupAttribute));

                    if(null == attribute) {
                        //Attribute not found, so no group was specified. Put the system into the default group, which is SimulationSystemGroup
                        attribute = new UpdateInGroupAttribute(typeof(SimulationSystemGroup));
                    }


                    //Ensure we are adding to a ComponentSystemGroup
                    if(!typeof(ComponentSystemGroup).IsAssignableFrom(attribute.GroupType)) {
                        throw new Exception($"Invalid Type in {systemType.Name} UpdateInGroupAttribute: {attribute.GroupType.Name}");
                    }

                    //Find or create the ComponentSystemGroup instance for the system to attach to
                    var desiredUpdateGroup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(attribute.GroupType);

                    //Spawn the system defined in the mod file
                    var systemInMod = World.DefaultGameObjectInjectionWorld.CreateSystem(systemType.RawType);

                    //Manually add the system to receive Update() calls
                    (desiredUpdateGroup as ComponentSystemGroup).AddSystemToUpdateList(systemInMod);
                }
            }
        }
    }
}
