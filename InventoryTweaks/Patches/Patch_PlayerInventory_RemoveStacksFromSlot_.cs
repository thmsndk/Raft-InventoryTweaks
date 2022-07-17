//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//namespace thmsn.InventoryTweaks.Patches
//{
//    /// <summary>
//    /// Replaces item with already existing item from inventory.
//    /// </summary>
//    [HarmonyPatch(typeof(PlayerInventory), "RemoveStacksFromSlot", typeof(Slot), typeof(int))]
//    class Patch_PlayerInventory_RemoveStacksFromSlot
//    {
//        // TODO: there seems to be an issue on death with duplication, either that, or when picking up really broken items.

//        static void Prefix(PlayerInventory __instance, Slot slot, int stacksToRemove, out HotBarSlotAndItemInstance __state)
//        {
//            //Debug.Log("RemoveStacksFromSlot");
//            __state = new HotBarSlotAndItemInstance
//            {
//                HotBarSlot = slot,
//                ItemInstance = slot.itemInstance
//            };
//            //csrun RAPI.GetLocalPlayer().Inventory.GetSelectedHotbarSlot().itemInstance.Uses = 1;
//            //Debug.Log($"{slot.itemInstance.UniqueName} uses {slot.itemInstance.Uses} / {slot.itemInstance.BaseItemMaxUses}");
//        }

//        static void Postfix(PlayerInventory __instance, int stacksToRemove, HotBarSlotAndItemInstance __state, bool __result)
//        {
//            if (__result)
//            {
//                // item broke
//                var player = RAPI.GetLocalPlayer();
//                var selectedHotBarSlot = __state.HotBarSlot;
//                var itemInstance = __state.ItemInstance;
                
//                foreach (Slot slot in player.Inventory.allSlots)
//                {
//                    if (slot.itemInstance != null && slot.itemInstance.UniqueName == itemInstance.UniqueName && slot != selectedHotBarSlot)
//                    {
//                        selectedHotBarSlot.SetItem(slot.itemInstance); // Assign the item from the slot to the hotbar;
//                        slot.Reset(); // Clear the slot of the item;
//                        break;
//                    }
//                }
//            }
//        }

//        private class HotBarSlotAndItemInstance
//        {
//            public ItemInstance ItemInstance { get; set; }
//            public Slot HotBarSlot { get; set; }
//        }
//    }
//}
