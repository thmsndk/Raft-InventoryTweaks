using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace thmsn.InventoryTweaks.Patches
{
    internal static class PatchHelpers
    {
        internal static readonly AccessTools.FieldRef<Slot, Inventory> Slot_inventory_Ref =
            AccessTools.FieldRefAccess<Slot, Inventory>(AccessTools.Field(typeof(Slot), "inventory"));

        // TODO: Move to AccessTools.MethodDelegate with Harmony 2.0.2
        internal static readonly FastInvokeHandler PlayerInventory_MoveSlotToEmpty =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(PlayerInventory), "MoveSlotToEmpty"));

        internal static readonly FastInvokeHandler PlayerInventory_SwitchSlots =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(PlayerInventory), "SwitchSlots"));

        internal static readonly FastInvokeHandler PlayerInventory_StackSlots =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(PlayerInventory), "StackSlots"));


        internal static bool IsSelectedHotbarSlot(Slot slot)
        {
            return Slot_inventory_Ref(slot) is PlayerInventory inventory && inventory.hotbar.IsSelectedHotSlot(slot);
        }

        internal static void ReplenishSlotIfNeeded(Slot slot, int originalUniqueIndex,
            PlayerInventory playerInventory = null)
        {
            if (slot.itemInstance?.UniqueIndex == originalUniqueIndex)
                return;

            var inventory = playerInventory ? playerInventory : Slot_inventory_Ref(slot) as PlayerInventory;
            if (inventory is null)
                return;

            var allSlots = inventory.allSlots;
            for (int index = inventory.hotslotCount, count = allSlots.Count; index < count; index++)
            {
                var localSlot = allSlots[index];

                if (localSlot.IsEmpty)
                    continue;

                var slotItemInstance = localSlot.itemInstance;
                if (slotItemInstance.UniqueIndex != originalUniqueIndex)
                    continue;

                if (slot.IsEmpty)
                    PlayerInventory_MoveSlotToEmpty(inventory,
                        new object[] { localSlot, slot, slotItemInstance.Amount });
                else
                    PlayerInventory_SwitchSlots(inventory, new object[] { localSlot, slot });
                break;
            }
        }
    }
}