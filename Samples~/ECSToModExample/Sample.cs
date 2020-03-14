using UnityEngine;
using Unity.Entities;
using UMod;

namespace Vestigial.Ecstoumod {
    /// <summary>
    /// How to use ECSToUMod
    /// </summary>
    public class SampleECSToUModUsage {

        //This [RuntimeInitializeOnLoadMethod] attribute causes the static method to run "automatically" during a certain phase
        //We need to load the mods into memory before RuntimeInitializeLoadType.BeforeSceneLoaded
        //so RuntimeInitializeLoadType.SubsystemRegistration is one option
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void LoadMods() {

            //In this example, load mods from /Assets
            Mod.DefaultDirectory = new ModDirectory("Assets");

            //Keep track of all the ModHost objects that result from loading
            var modHosts =  Mod.LoadAll();

            //Important! Pass the ModHosts to the ECSToUMod class
            ECSToUMod.ModHosts = modHosts;
        }

        //At some point in the future, we will have a scene running, and want to spawn the ECS systems
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void SpawnSystems() {
            ECSToUMod.LoadSystemsFromModHosts();
        }
    }
}