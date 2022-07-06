using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace thmsn.InventoryTweaks.Patches
{
    /// <summary>
    ///     Patches <see cref="Inventory.RemoveItem">Inventory.RemoveItem</see> to replenish items if needed
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.RemoveItem))]
    internal static class Inventory_RemoveItem_Patch
    {
        private static void Prefix(Inventory __instance, out int __state)
        {
            __state = (__instance as PlayerInventory)?.GetSelectedHotbarItem()?.UniqueIndex ?? -1;
        }

        private static void Postfix(Inventory __instance, int __state)
        {
            if (__state == -1 || !(__instance is PlayerInventory playerInventory))
                return;

            PatchHelpers.ReplenishSlotIfNeeded(playerInventory.GetSelectedHotbarSlot(), __state);
        }
    }
}