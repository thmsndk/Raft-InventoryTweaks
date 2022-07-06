using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming

namespace thmsn.InventoryTweaks.Patches
{
    /// <summary>
    ///     Moves all items of same type as <see cref="Slot" /> item when clicked while holding Left Ctrl and Left Shift
    /// </summary>
    [HarmonyPatch(typeof(Slot), "OnPointerDown")]
    internal class Slot_OnPointerDown_Patch
    {
        private static bool Prefix(Slot __instance, PointerEventData eventData)
        {
            if (__instance.IsEmpty || !(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))
                return true;

            var inventory = PatchHelpers.Slot_inventory_Ref(__instance);

            if (inventory.secondInventory is Inventory_ResearchTable)
                return true;

            var allSlots = inventory.allSlots;
            int index = 0,
                end = allSlots.Count;

            // from/to hotbar
            if (inventory.secondInventory == null && inventory is PlayerInventory playerInventory)
            {
                int hotslotCount = playerInventory.hotslotCount;
                if (playerInventory.hotbar.ContainsSlot(__instance))
                    end = hotslotCount;
                else
                    index = hotslotCount;
            }

            int uniqueIndex = __instance.itemInstance.UniqueIndex;
            for (; index < end; index++)
            {
                var slot = allSlots[index];

                if (slot.itemInstance?.UniqueIndex != uniqueIndex)
                    continue;

                inventory.ShiftMoveItem(slot, eventData);

                if (slot.itemInstance != null)
                    break;
            }

            return false;
        }
    }
}