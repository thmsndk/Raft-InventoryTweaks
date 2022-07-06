using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace thmsn.InventoryTweaks
{
    public class InventoryTweaksMod : Mod
    {
        public const string ModNamePrefix = "<color=white>Inventory</color><color=green>Tweaks</color>";
        private const string HarmonyId = "com.thmsn.inventory-tweaks";
        private Harmony harmony;

        public void Start()
        {
            harmony = new Harmony(HarmonyId);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Debug.Log(ModNamePrefix + " has been loaded!");
        }

        public void OnModUnload()
        {
            harmony.UnpatchAll(HarmonyId);
            Debug.Log(ModNamePrefix + " has been unloaded!");
        }
    }
}