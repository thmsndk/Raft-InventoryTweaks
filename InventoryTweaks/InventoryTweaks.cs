using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace thmsn.InventoryTweaks
{
    public class InventoryTweaksMod : Mod
    {
        public static string ModNamePrefix = "<color=white>Inventory</color><color=green>Tweaks</color>";
        private const string harmonyId = "com.thmsn.inventory-tweaks";
        Harmony harmony;
        public void Start()
        {
            harmony = new Harmony(harmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Debug.Log(ModNamePrefix + " has been loaded!");
        }

        public void OnModUnload()
        {
            harmony.UnpatchAll(harmonyId);
            Debug.Log(ModNamePrefix + " has been unloaded!");
        }
    }
}